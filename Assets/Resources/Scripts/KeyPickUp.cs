using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickUp : MonoBehaviour
{
    // Reference au joueur 
    public PlayerStats playerStats;
    // Text d'indication de la possibilit� de ramasser la cl�
    public GameObject ui;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Debug.Log("Le joueur � touch� la cl�.");
            ui.SetActive(true);
            playerStats.canTakeKey = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Debug.Log("Le joueur � touch� la cl�.");
            ui.SetActive(false);
            playerStats.canTakeKey = false;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, range);
    //}
}
