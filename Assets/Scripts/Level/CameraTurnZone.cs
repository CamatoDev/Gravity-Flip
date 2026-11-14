using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CameraTurnZone : MonoBehaviour
{
    [Header("Caméra")]
    [Tooltip("Angle Y de la caméra pour le sens Aller")]
    public float cameraAngleForward = 90f;
    [Tooltip("Angle Y de la caméra pour le sens Retour")]
    public float cameraAngleBackward = 0f;

    [Header("Joueur (Piste 2.5D)")]
    [Tooltip("Calcul automatique : l'angle de piste du joueur est déduit pour que 'avancer' aille toujours à gauche de la vue caméra")]
    public bool autoPlayerAngle = true;

    [Tooltip("Angle Y manuel de la piste du joueur (Sens Aller) si autoPlayerAngle est décoché")]
    public float playerAngleForward = 0f;
    [Tooltip("Angle Y manuel de la piste du joueur (Sens Retour) si autoPlayerAngle est décoché")]
    public float playerAngleBackward = -90f;

    private BoxCollider _boxCollider;

    void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _boxCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerScript playerScript = other.GetComponent<PlayerScript>();
        CameraFollow2D camFollow = Camera.main != null ? Camera.main.GetComponent<CameraFollow2D>() : null;

        // Détection infaillible de sens basée sur l'angle actuel de la caméra (ponytail: évite tout souci de produit scalaire)
        bool isCurrentlyForward = camFollow != null && Mathf.Abs(Mathf.DeltaAngle(camFollow.targetAngleY, cameraAngleForward)) < 30f;

        float targetCam = isCurrentlyForward ? cameraAngleBackward : cameraAngleForward;
        float targetPlayer = autoPlayerAngle ? (targetCam - 90f) : (isCurrentlyForward ? playerAngleBackward : playerAngleForward);

        if (camFollow != null) camFollow.SetCameraAngleY(targetCam);
        if (playerScript != null) playerScript.SetTrackAngle(targetPlayer);
    }

    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        Vector3 size = box != null ? box.size : Vector3.one;
        Vector3 center = box != null ? box.center : Vector3.zero;

        Gizmos.color = new Color(0f, 0.7f, 1f, 0.35f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(center, size);
        Gizmos.DrawWireCube(center, size);

        // Flèche bleue montrant la direction "Sens Aller"
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(center, Vector3.forward * (size.z * 0.75f + 1f));
    }
}
