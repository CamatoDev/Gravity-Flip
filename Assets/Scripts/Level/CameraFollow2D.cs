using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Cible à suivre")]
    public Transform target;

    [Header("Paramètres de Suivi")]
    [Tooltip("Décalage de la caméra par rapport au joueur")]
    public Vector3 offset = new Vector3(0f, 1.75f, -5f);

    [Tooltip("Temps d'amortissement pour le mouvement de position")]
    public float smoothTime = 0.2f;

    [Tooltip("Temps d'amortissement pour la rotation")]
    public float rotationSmoothTime = 0.25f;

    [Header("Orientation de la Piste (Virages)")]
    public float targetAngleY = 0f;

    private Vector3 _currentVelocity;
    private Quaternion _targetRotation;

    void Start()
    {
        // Recherche automatique du joueur s'il n'est pas assigné dans l'Inspecteur
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }

        targetAngleY = transform.eulerAngles.y;
        _targetRotation = Quaternion.Euler(transform.eulerAngles.x, targetAngleY, 0f);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calcul de la rotation cible
        _targetRotation = Quaternion.Euler(transform.eulerAngles.x, targetAngleY, 0f);

        // Position désirée en tenant compte de la rotation de la vue
        Vector3 rotatedOffset = _targetRotation * offset;
        Vector3 desiredPosition = target.position + rotatedOffset;

        // Amortissement fluide de la position (SmoothDamp)
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _currentVelocity, smoothTime);

        // Rotation fluide de la vue lors des virages
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, Time.deltaTime / Mathf.Max(0.01f, rotationSmoothTime));
    }

    /// <summary>
    /// Permet aux triggers de virage d'orienter la caméra de manière fluide
    /// </summary>
    public void SetCameraAngleY(float newAngleY)
    {
        targetAngleY = newAngleY;
    }
}
