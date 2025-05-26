using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    // Réference au joueur
    PlayerStats _player;
    // UI d'indication 
    public GameObject indication;
    // Distance à laquel le joueur peut ouvrir la porte
    public float distanceToOpen = 2f;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        indication.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Calcule de la distance entre le joueur et la porte
        float Distance = Vector3.Distance(_player.transform.position, transform.position);

        if(Distance <= distanceToOpen && _player.haveKey)
        {
            Debug.Log("Ouverture de la porte...");
            Destroy(gameObject);
        }
        if(Distance <= distanceToOpen)
        {
            indication.SetActive(true);
        }
        if (Distance > distanceToOpen)
        {
            indication.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanceToOpen);
    }
}
