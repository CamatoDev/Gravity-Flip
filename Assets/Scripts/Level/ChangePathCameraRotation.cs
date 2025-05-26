using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePathCameraRotation : MonoBehaviour
{
    private bool _pathChanged = false;
    Transform _player;
    public float distanceToActive = 1f;
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _pathChanged = false;
    }

    // Update is called once per frame
    void Update()
    {
        //float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        //if(distanceToPlayer < distanceToActive && !_pathChanged)
        //{
        //    camera.RotateAround(player.position, Vector3.up, 90);
        //    _pathChanged = true;
        //}
        //if (distanceToPlayer < distanceToActive && _pathChanged)
        //{
        //    camera.RotateAround(player.position, Vector3.up, -90);
        //    _pathChanged = false;
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!_pathChanged)
            {
                _player.Rotate(Vector3.up, 90);
            }
            if (_pathChanged)
            {
                _player.Rotate(Vector3.up, -90);
            }

            _pathChanged = !_pathChanged;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanceToActive);
    }
}
