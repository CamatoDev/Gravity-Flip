using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerGravityFlip : MonoBehaviour
{
    [Header("Paramètres Gravité (Inactif pour le moment)")]
    public float gravityMagnitude = 18f;
    public bool isFlipped { get; private set; } = false;

    [Header("Feedback Visuel & Audio")]
    public Transform visualModel;
    public AudioSource audioSource;
    public AudioClip flipSound;
    public AudioClip landSound;
    public ParticleSystem landParticles;

    [Header("Détection d'Atterrissage Lourd (Hard Landing)")]
    [Tooltip("Seuil de vitesse verticale de chute pour déclencher l'impact lourd (ex: 10)")]
    public float hardLandingVelocityThreshold = 10f;

    [Tooltip("Durée de la pause de réception en secondes pour donner du poids à la chute")]
    public float recoveryDuration = 0.35f;

    private Rigidbody _rb;
    private PlayerScript _playerScript;
    private Animator _animator;
    private float _lastVerticalVelocity = 0f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _playerScript = GetComponent<PlayerScript>();
        _animator = GetComponent<Animator>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Start()
    {
        // Chargement automatique d'un son d'impact de pas lourd parmi les fichiers du jeu si non assigné
        if (landSound == null)
        {
            landSound = Resources.Load<AudioClip>("Sounds/Footsteps - Essentials/Footsteps_Tile/Footsteps_Tile_Jump/Footsteps_Tile_Jump_Land_01");
            if (landSound == null)
            {
                landSound = Resources.Load<AudioClip>("Sounds/Footsteps - Essentials/Footsteps_Rock/Footsteps_Rock_Jump/Footsteps_Rock_Jump_Land_01");
            }
        }
    }

    void FixedUpdate()
    {
        // 1. Mémorisation de la vitesse maximale atteinte lors de la chute
        if (_playerScript != null && !_playerScript.isGrounded)
        {
            if (_rb.velocity.y < -1f)
            {
                _lastVerticalVelocity = Mathf.Max(_lastVerticalVelocity, Mathf.Abs(_rb.velocity.y));
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Détecte le contact avec le sol suite à une chute rapide
        if (_lastVerticalVelocity >= hardLandingVelocityThreshold)
        {
            if (collision.contacts.Length > 0 && collision.contacts[0].normal.y > 0.5f)
            {
                TriggerLandingImpact();
            }
        }
        _lastVerticalVelocity = 0f;
    }

    /// <summary>
    /// Déclenche le feedback complet d'atterrissage lourd :
    /// Animation Hard Landing, son d'impact, particules et court temps de pause
    /// </summary>
    public void TriggerLandingImpact()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("hardLand");
        }

        if (landParticles != null)
        {
            landParticles.Play();
        }

        if (audioSource != null && landSound != null)
        {
            audioSource.PlayOneShot(landSound, 0.85f);
        }

        if (_playerScript != null && recoveryDuration > 0f)
        {
            _playerScript.TriggerHardLandingPause(recoveryDuration);
        }
    }

    /// <summary>
    /// Mécanique d'inversion de gravité (prête pour la phase ultérieure)
    /// </summary>
    public void FlipGravity()
    {
        isFlipped = !isFlipped;

        if (visualModel != null)
        {
            Vector3 rot = visualModel.localEulerAngles;
            rot.z = isFlipped ? 180f : 0f;
            visualModel.localEulerAngles = rot;
        }

        if (audioSource != null && flipSound != null)
        {
            audioSource.PlayOneShot(flipSound);
        }
    }
}
