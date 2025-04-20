using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public enum MovementAxis { Horizontal, Vertical }
    [Header("Type de mouvement")]
    public MovementAxis axis = MovementAxis.Horizontal;

    [Header("Limites de déplacement")]
    public float minOffset = -2f;
    public float maxOffset = 2f;

    [Header("Vitesse")]
    public float speed = 2f;

    private Vector3 _startPos;

    void Start()
    {
        _startPos = transform.position;
    }

    void Update()
    {
        // Calcule une valeur oscillante entre minOffset et maxOffset
        float range = maxOffset - minOffset;
        float pingPong = Mathf.PingPong(Time.time * speed, range) + minOffset;

        Vector3 newPos = _startPos;
        if (axis == MovementAxis.Horizontal)
            newPos.x += pingPong;
        else
            newPos.y += pingPong;

        transform.position = newPos;
    }
}
