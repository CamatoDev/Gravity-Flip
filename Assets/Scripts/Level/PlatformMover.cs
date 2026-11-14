using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlatformMover : MonoBehaviour
{
    public enum MovementAxis { Horizontal, Vertical }

    [Header("Type de mouvement (Mode Offset)")]
    public MovementAxis axis = MovementAxis.Horizontal;

    [Header("Limites de déplacement")]
    public float minOffset = -2f;
    public float maxOffset = 2f;

    [Header("Vitesse")]
    public float speed = 2f;

    [Header("Points de passage optionnels (Mode Waypoints)")]
    [Tooltip("Si pointA et pointB sont assignés, la plateforme suit ces cibles au lieu de l'offset")]
    public Transform pointA;
    public Transform pointB;
    public float waitTimeAtPoint = 0.5f;

    [Header("Inertie au saut")]
    [Tooltip("Facteur multiplicateur de transfert de vitesse au saut")]
    public float inertiaMultiplier = 1f;

    public Vector3 CurrentVelocity { get; private set; }

    private Rigidbody _rb;
    private Vector3 _startPos;
    private Vector3 _targetPos;
    private float _waitTimer = 0f;

    // Gestion de l'adhérence continue (élimine les micro-décrochages et oscillations)
    private Rigidbody _carriedPlayerRb;
    private PlayerScript _carriedPlayerScript;
    private float _contactCooldown = 0f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            _rb = gameObject.AddComponent<Rigidbody>();
        }
        _rb.isKinematic = true;
        _rb.useGravity = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Start()
    {
        _startPos = transform.position;

        if (pointA != null && pointB != null)
        {
            transform.position = pointA.position;
            _targetPos = pointB.position;
        }
    }

    void FixedUpdate()
    {
        Vector3 currentPos = transform.position;
        Vector3 newPos = currentPos;

        if (pointA != null && pointB != null)
        {
            // Mode Waypoints
            if (_waitTimer > 0f)
            {
                _waitTimer -= Time.fixedDeltaTime;
                CurrentVelocity = Vector3.zero;
                if (_carriedPlayerScript != null)
                {
                    _carriedPlayerScript.SetPlatformVelocity(Vector3.zero, this);
                }
                return;
            }

            newPos = Vector3.MoveTowards(currentPos, _targetPos, speed * Time.fixedDeltaTime);

            if (Vector3.Distance(newPos, _targetPos) < 0.02f)
            {
                _targetPos = (_targetPos == pointA.position) ? pointB.position : pointA.position;
                _waitTimer = waitTimeAtPoint;
            }
        }
        else
        {
            // Mode Offset (Ping-Pong) rétrocompatible
            float range = maxOffset - minOffset;
            float pingPong = Mathf.PingPong(Time.time * speed, range) + minOffset;

            newPos = _startPos;
            if (axis == MovementAxis.Horizontal)
                newPos.x += pingPong;
            else
                newPos.y += pingPong;
        }

        Vector3 deltaPos = newPos - currentPos;

        // Filtrage strict de la vitesse selon l'axe de déplacement (élimine tout parasite avant/arrière)
        Vector3 rawVelocity = deltaPos / Time.fixedDeltaTime;
        if (pointA == null || pointB == null)
        {
            if (axis == MovementAxis.Vertical)
                CurrentVelocity = new Vector3(0f, rawVelocity.y, 0f);
            else
                CurrentVelocity = new Vector3(rawVelocity.x, 0f, 0f);
        }
        else
        {
            CurrentVelocity = rawVelocity;
        }

        // Déplacement physique cinématique
        _rb.MovePosition(newPos);

        // Décrémentation du cooldown d'adhérence
        if (_contactCooldown > 0f)
        {
            _contactCooldown -= Time.fixedDeltaTime;
            if (_contactCooldown <= 0f && _carriedPlayerRb != null)
            {
                DetachPlayer();
            }
        }

        // Entraînement direct par vélocité physique (élimine les conflits d'interpolation et vibrations)
        if (_carriedPlayerScript != null)
        {
            _carriedPlayerScript.SetPlatformVelocity(CurrentVelocity, this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.transform.position.y > transform.position.y - 0.2f)
            {
                AttachPlayer(other.gameObject);
                _contactCooldown = 0.2f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DetachPlayer();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (collision.transform.position.y > transform.position.y - 0.2f)
            {
                AttachPlayer(collision.gameObject);
                _contactCooldown = 0.2f;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            // Démarre une courte tolérance (0.15s) pour éviter les micro-décrochages à chaque frame
            _contactCooldown = 0.15f;
        }
    }

    private void AttachPlayer(GameObject playerObj)
    {
        if (_carriedPlayerRb == null)
        {
            _carriedPlayerRb = playerObj.GetComponent<Rigidbody>();
            _carriedPlayerScript = playerObj.GetComponent<PlayerScript>();
        }
    }

    private void DetachPlayer()
    {
        if (_carriedPlayerRb == null) return;

        if (_carriedPlayerScript != null)
        {
            _carriedPlayerScript.ClearPlatformVelocity(this);
        }

        // Ne transfère l'inertie que si le joueur a sauté ou quitté la plateforme en l'air
        bool isLeavingInAir = _carriedPlayerRb.velocity.y > 0.5f || (_carriedPlayerScript != null && !_carriedPlayerScript.isGrounded);

        if (CurrentVelocity.sqrMagnitude > 0.01f && isLeavingInAir)
        {
            Vector3 inertia = CurrentVelocity * inertiaMultiplier;

            // Boost vertical lors du saut sur plateforme montante
            if (inertia.y > 0.05f && _carriedPlayerRb.velocity.y > 0.5f)
            {
                _carriedPlayerRb.velocity = new Vector3(_carriedPlayerRb.velocity.x, _carriedPlayerRb.velocity.y + inertia.y, _carriedPlayerRb.velocity.z);
            }

            // Élan horizontal
            if (Mathf.Abs(inertia.x) > 0.05f || Mathf.Abs(inertia.z) > 0.05f)
            {
                Vector3 horizontalInertia = new Vector3(inertia.x, 0f, inertia.z);
                if (_carriedPlayerScript != null)
                {
                    _carriedPlayerScript.AddExternalMomentum(horizontalInertia);
                }
                else
                {
                    _carriedPlayerRb.velocity += horizontalInertia;
                }
            }
        }

        _carriedPlayerRb = null;
        _carriedPlayerScript = null;
        _contactCooldown = 0f;
    }
}
