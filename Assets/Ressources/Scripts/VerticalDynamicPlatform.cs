using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalDynamicPlatform : MonoBehaviour
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
        maxDistance = transform.position.y + offset;
        minDistance = transform.position.y;
        //StartCoroutine(MovePlatform());
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y >= minDistance && transform.position.y < maxDistance)
        {
            transform.position += transform.up * Time.deltaTime * 2;
        }
    }
}
