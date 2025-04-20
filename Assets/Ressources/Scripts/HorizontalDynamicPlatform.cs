using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HorizontalDynamicPlatform : MonoBehaviour
{
    // Distance max et min de deplacement de la platforme
    public float maxDistance;
    public float minDistance;
    // Valeur du deplacement
    public float offset = 2f;
    // Delai avant de changement de sens
    public float cooldown = 2f;
    // Start is called before the first frame update
    void Start()
    {
        maxDistance = transform.position.x - offset;
        minDistance = transform.position.x;
        //StartCoroutine(MovePlatform());
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x <= minDistance && transform.position.x > maxDistance)
        {
            transform.position -= transform.right * Time.deltaTime * 2;
        }
    }

    //public IEnumerator MovePlatform()
    //{
    //    transform.position -= transform.right * Time.deltaTime * 2;
    //    yield return cooldown;
    //    transform.position += transform.right * Time.deltaTime * 2;
    //}
}
