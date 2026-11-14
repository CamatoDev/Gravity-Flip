using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Points de passage")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 2.5f;

    [Header("Comportement")]
    public float waitTimeAtPoint = 0.5f;

    [Header("Inertie au saut")]
    [Tooltip("Facteur multiplicateur de transfert de vitesse au saut")]
    public float inertiaMultiplier = 1f;

    public Vector3 CurrentVelocity { get; private set; }

    private Rigidbody _rb;
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
        if (pointA != null)
        {
            transform.position = pointA.position;
            _targetPos = (pointB != null) ? pointB.position : pointA.position;
        }
        else if (pointB != null)
        {
            _targetPos = pointB.position;
        }
        else
        {
            Debug.LogWarning($"[MovingPlatform] Aucun pointA ou pointB configuré sur {gameObject.name} !");
            _targetPos = transform.position;
        }
    }

    void FixedUpdate()
    {
        if (pointA == null && pointB == null) return;

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

        Vector3 currentPos = transform.position;
        Vector3 newPos = Vector3.MoveTowards(currentPos, _targetPos, speed * Time.fixedDeltaTime);
        Vector3 deltaPos = newPos - currentPos;

        CurrentVelocity = deltaPos / Time.fixedDeltaTime;

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

        // Inversion de direction quand le point cible est atteint
        if (Vector3.Distance(newPos, _targetPos) < 0.02f)
        {
            if (pointA != null && pointB != null)
            {
                _targetPos = (_targetPos == pointA.position) ? pointB.position : pointA.position;
            }
            _waitTimer = waitTimeAtPoint;
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

            if (inertia.y > 0.05f && _carriedPlayerRb.velocity.y > 0.5f)
            {
                _carriedPlayerRb.velocity = new Vector3(_carriedPlayerRb.velocity.x, _carriedPlayerRb.velocity.y + inertia.y, _carriedPlayerRb.velocity.z);
            }

            Vector3 horizontalInertia = new Vector3(inertia.x, 0f, inertia.z);
            if (horizontalInertia.sqrMagnitude > 0.01f)
            {
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
