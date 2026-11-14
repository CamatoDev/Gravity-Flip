using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerScript : MonoBehaviour
{
    private Animator playerAnimator;
    private Rigidbody rb;
    private AudioSource playerAudioSource;
    private Collider playerCollider;
    private PlayerStats playerStats;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip runSound;
    [SerializeField] private AudioClip danceSound;
    [SerializeField] private AudioClip turnSound;

    [Header("Vitesses & Physique")]
    [SerializeField] public float walkSpeed = 3.5f;
    [SerializeField] public float runSpeed = 6f;
    [SerializeField] public float rotationSpeed = 720f;
    [SerializeField] public float jumpForce = 250f;
    [SerializeField] public float jumpCoolDown = 0.5f;

    [Header("Orientation & Verrouillage 2.5D")]
    [Tooltip("Demi-tour instantané à 180° lors du changement de direction")]
    public bool instantRotation = true;
    [Tooltip("Angle Y de la trajectoire (ex: -90 pour Level 1)")]
    public float currentTrackAngle = -90f;
    [Tooltip("Inverser l'axe de déplacement si nécessaire")]
    public bool invertAxis = false;
    [Tooltip("Verrouiller la profondeur pour empêcher tout glissement hors de la ligne de course")]
    public bool lockDepth = true;
    [Tooltip("Coordonnée de profondeur verrouillée")]
    public float lockedDepth = 0f;

    [Header("Ground Check Settings (Anti-Blocage Bordure)")]
    [SerializeField] private float groundCheckRadius = 0.25f;
    [SerializeField] private Vector3 groundCheckOffset = new Vector3(0f, -0.08f, 0f);
    [Tooltip("Écartement des points de détection avant/arrière pour détecter le sol même avec un seul pied sur le bord")]
    [SerializeField] private float edgeFootDistance = 0.25f;
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("Délai de grâce permettant de sauter juste après avoir quitté une plateforme (Coyote Time)")]
    public float coyoteTime = 0.15f;

    public bool isGrounded { get; private set; }
    public float jumpCooling = 0f;

    private const float timeOut = 15f;
    private float countDown = timeOut;
    private float inputMove;
    private bool jumpRequested = false;
    private float coyoteCounter = 0f;
    private Quaternion targetRotation;

    void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        playerAudioSource = GetComponent<AudioSource>();
        playerCollider = GetComponent<Collider>();
        playerStats = GetComponent<PlayerStats>();

        // 1. DÉSACTIVER impérativement le Root Motion pour éviter tout glissement par les animations
        if (playerAnimator != null)
        {
            playerAnimator.applyRootMotion = false;
        }

        // 2. Configuration par défaut du CapsuleCollider (calibrée en Play Mode)
        if (playerCollider is CapsuleCollider capsule)
        {
            capsule.center = new Vector3(0f, 0.4512336f, 0f);
            capsule.radius = 0.2795277f;
            capsule.height = 1.476729f;
            capsule.direction = 1; // Axe Y
        }

        // 3. MATÉRIAU SANS FRICTION : Empêche le joueur de rester collé aux parois verticales
        if (playerCollider != null)
        {
            PhysicMaterial frictionless = new PhysicMaterial("PlayerZeroFriction")
            {
                dynamicFriction = 0f,
                staticFriction = 0f,
                frictionCombine = PhysicMaterialCombine.Minimum,
                bounceCombine = PhysicMaterialCombine.Minimum
            };
            playerCollider.material = frictionless;
        }

        // 4. Verrouillage PhysX natif via constraints (bloque l'axe de profondeur directement dans le moteur physique)
        UpdateConstraints();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // 5. Initialisation de l'orientation et de la coordonnée de profondeur de départ
        targetRotation = Quaternion.Euler(0f, currentTrackAngle, 0f);
        transform.rotation = targetRotation;

        Vector3 trackForward = Quaternion.Euler(0f, currentTrackAngle, 0f) * Vector3.forward;
        if (Mathf.Abs(trackForward.x) > 0.5f)
        {
            lockedDepth = transform.position.z;
        }
        else
        {
            lockedDepth = transform.position.x;
        }
    }

    void Update()
    {
        // Si le joueur est mort / tombé : couper immédiatement tout input, animation et son
        if (playerStats != null && (playerStats.isFalled || playerStats.currentLife <= 0f))
        {
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("walk", false);
                playerAnimator.SetBool("walkBack", false);
                playerAnimator.SetFloat("run", 0f);
            }
            if (playerAudioSource != null && playerAudioSource.isPlaying)
            {
                playerAudioSource.Stop();
            }
            return;
        }

        // 1. Lecture de l'input horizontal (-1 = gauche/arrière, +1 = droite/avant)
        float rawHorizontal = Input.GetAxisRaw("Horizontal");
        inputMove = invertAxis ? -rawHorizontal : rawHorizontal;

        // 2. Demi-tour 180° selon le sens d'avancement (instantané par défaut)
        if (inputMove > 0.05f)
        {
            targetRotation = Quaternion.Euler(0f, currentTrackAngle, 0f);
            if (instantRotation)
            {
                rb.rotation = targetRotation;
                transform.rotation = targetRotation;
            }
        }
        else if (inputMove < -0.05f)
        {
            targetRotation = Quaternion.Euler(0f, currentTrackAngle + 180f, 0f);
            if (instantRotation)
            {
                rb.rotation = targetRotation;
                transform.rotation = targetRotation;
            }
        }

        // 3. Rotation lissée uniquement si instantRotation est désactivé
        if (!instantRotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 4. Gestion des animations et audio de marche/course
        bool isMoving = Mathf.Abs(inputMove) > 0.05f;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Sprint");

        // Tolérance d'animation au sol : évite toute coupure brutale de l'animation lors du passage sur les joints ou raccords de blocs
        bool shouldPlayWalk = isMoving && (isGrounded || coyoteCounter > 0f || Mathf.Abs(rb.velocity.y) < 1.2f);

        if (shouldPlayWalk)
        {
            countDown = timeOut;
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("dance", false);
                playerAnimator.SetBool("walkBack", false);

                if (isSprinting)
                {
                    playerAnimator.SetBool("walk", false);
                    playerAnimator.SetFloat("run", 1f);
                    if (playerAudioSource != null) playerAudioSource.clip = runSound;
                }
                else
                {
                    playerAnimator.SetBool("walk", true);
                    playerAnimator.SetFloat("run", 0f);
                    if (playerAudioSource != null) playerAudioSource.clip = walkSound;
                }
            }

            if (playerAudioSource != null && !playerAudioSource.isPlaying && playerAudioSource.clip != null)
            {
                playerAudioSource.loop = true;
                playerAudioSource.volume = 1f;
                playerAudioSource.Play();
            }
        }
        else
        {
            // Dans les airs ou à l'arrêt : on coupe la marche au sol
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("walk", false);
                playerAnimator.SetBool("walkBack", false);
                playerAnimator.SetFloat("run", 0f);
            }

            if (playerAudioSource != null && (playerAudioSource.clip == walkSound || playerAudioSource.clip == runSound))
            {
                playerAudioSource.Stop();
            }

            // Gestion de l'animation Dance au repos prolongé
            if (!isMoving && isGrounded)
            {
                countDown -= Time.deltaTime;
                if (countDown <= 0f)
                {
                    if (playerAnimator != null) playerAnimator.SetBool("dance", true);
                    if (playerAudioSource != null)
                    {
                        playerAudioSource.clip = danceSound;
                        playerAudioSource.loop = true;
                        playerAudioSource.volume = 0.25f;
                        if (!playerAudioSource.isPlaying && danceSound != null)
                        {
                            playerAudioSource.Play();
                        }
                    }
                    countDown = timeOut;
                }
            }
            else
            {
                countDown = timeOut;
                if (playerAnimator != null) playerAnimator.SetBool("dance", false);
            }
        }

        // 5. Capture de l'intention de saut
        if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("JoystickJump"))
        {
            jumpRequested = true;
            if (playerAnimator != null) playerAnimator.SetBool("dance", false);
            countDown = timeOut;
        }
    }

    void FixedUpdate()
    {
        // Si le joueur est mort : stopper toute vélocité horizontale
        if (playerStats != null && (playerStats.isFalled || playerStats.currentLife <= 0f))
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            return;
        }

        // 1. Détection physique du sol multi-points (Anti-Blocage Bordure & Joints)
        GroundCheck();

        // 2. Calcul de la vitesse désirée
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetButton("Sprint");
        float currentSpeed = isSprinting ? runSpeed : walkSpeed;

        // Vitesse rigoureusement alignée sur l'axe de piste
        Vector3 trackForward = Quaternion.Euler(0f, currentTrackAngle, 0f) * Vector3.forward;
        float moveMagnitude = (inputMove > 0.05f) ? currentSpeed : (inputMove < -0.05f ? -currentSpeed : 0f);
        Vector3 moveVelocity = trackForward * moveMagnitude;

        // 3. Application de la vélocité horizontale (la profondeur est verrouillée nativement par les RigidbodyConstraints sans toucher à rb.position)
        if (Mathf.Abs(trackForward.x) > 0.5f)
        {
            rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, 0f);
        }
        else
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, moveVelocity.z);
        }

        // 4. Exécution du saut avec Coyote Time
        jumpCooling += Time.fixedDeltaTime;
        if (jumpRequested)
        {
            if ((isGrounded || coyoteCounter > 0f) && jumpCooling > jumpCoolDown)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(Vector3.up * jumpForce);

                if (playerAnimator != null) playerAnimator.SetTrigger("jump");
                if (playerAudioSource != null && jumpSound != null)
                {
                    playerAudioSource.PlayOneShot(jumpSound);
                }
                jumpCooling = 0f;
                coyoteCounter = 0f; // Consommé
            }
            jumpRequested = false;
        }
    }

    private void UpdateConstraints()
    {
        Vector3 trackForward = Quaternion.Euler(0f, currentTrackAngle, 0f) * Vector3.forward;
        if (Mathf.Abs(trackForward.x) > 0.5f)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                             RigidbodyConstraints.FreezeRotationY | 
                             RigidbodyConstraints.FreezeRotationZ | 
                             RigidbodyConstraints.FreezePositionZ;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.FreezeRotationX | 
                             RigidbodyConstraints.FreezeRotationY | 
                             RigidbodyConstraints.FreezeRotationZ | 
                             RigidbodyConstraints.FreezePositionX;
        }
    }

    private LayerMask GetEffectiveGroundMask()
    {
        int playerLayer = gameObject.layer;
        LayerMask mask = (groundLayer.value == 0)
            ? ~LayerMask.GetMask("Ignore Raycast")
            : (groundLayer | (1 << 0) | (1 << 6));

        // Exclure le calque du joueur pour ne jamais s'auto-détecter
        return mask & ~(1 << playerLayer);
    }

    private Vector3 GetFootPosition()
    {
        if (playerCollider is CapsuleCollider capsule)
        {
            return transform.position - Vector3.up * (capsule.height * 0.5f - capsule.center.y);
        }
        else if (playerCollider != null)
        {
            return playerCollider.bounds.center - Vector3.up * playerCollider.bounds.extents.y;
        }
        return transform.position;
    }

    /// <summary>
    /// Détection de sol multi-points robuste :
    /// Calcule dynamiquement le bas réel de la capsule collider et applique une marge de détection
    /// pour éliminer tout décrochage au passage des joints entre plateformes.
    /// </summary>
    void GroundCheck()
    {
        Vector3 trackForward = Quaternion.Euler(0f, currentTrackAngle, 0f) * Vector3.forward;
        Vector3 footCenter = GetFootPosition();
        LayerMask mask = GetEffectiveGroundMask();

        // Rayon généreux (au moins 0.28m) placé légèrement au-dessus du sol pour garantir une pénétration d'au moins 10-15cm
        float checkRadius = Mathf.Max(groundCheckRadius, 0.28f);
        Vector3 checkCenter = footCenter + Vector3.up * (checkRadius * 0.5f);

        bool centerHit = Physics.CheckSphere(checkCenter, checkRadius, mask);
        bool frontHit = Physics.CheckSphere(checkCenter + trackForward * edgeFootDistance, checkRadius, mask);
        bool backHit = Physics.CheckSphere(checkCenter - trackForward * edgeFootDistance, checkRadius, mask);

        // Raycast laser de sécurité directement sous les pieds
        bool rayHit = Physics.Raycast(footCenter + Vector3.up * 0.15f, Vector3.down, 0.35f, mask);

        isGrounded = centerHit || frontHit || backHit || rayHit;

        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.fixedDeltaTime;
        }
    }

    /// <summary>
    /// Réoriente la piste lors d'un virage (ex: au tournant de Level 1)
    /// </summary>
    public void SetTrackAngle(float newAngle)
    {
        currentTrackAngle = newAngle;
        UpdateConstraints();
        Vector3 newTrackForward = Quaternion.Euler(0f, currentTrackAngle, 0f) * Vector3.forward;

        if (Mathf.Abs(newTrackForward.x) > 0.5f)
        {
            lockedDepth = transform.position.z;
        }
        else
        {
            lockedDepth = transform.position.x;
        }

        // Réoriente immédiatement le modèle vers l'avant ou l'arrière de la nouvelle piste
        targetRotation = Quaternion.Euler(0f, (inputMove >= 0) ? currentTrackAngle : currentTrackAngle + 180f, 0f);
        if (instantRotation)
        {
            rb.rotation = targetRotation;
            transform.rotation = targetRotation;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 trackForward = Quaternion.Euler(0f, currentTrackAngle, 0f) * Vector3.forward;
        Vector3 footCenter = GetFootPosition();
        float checkRadius = Mathf.Max(groundCheckRadius, 0.28f);
        Vector3 checkCenter = footCenter + Vector3.up * (checkRadius * 0.5f);

        Gizmos.color = isGrounded ? new Color(0, 1, 0, 0.5f) : new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(checkCenter, checkRadius);
        Gizmos.DrawSphere(checkCenter + trackForward * edgeFootDistance, checkRadius * 0.85f);
        Gizmos.DrawSphere(checkCenter - trackForward * edgeFootDistance, checkRadius * 0.85f);
        Gizmos.DrawLine(footCenter + Vector3.up * 0.15f, footCenter + Vector3.up * 0.15f + Vector3.down * 0.35f);
    }
}
