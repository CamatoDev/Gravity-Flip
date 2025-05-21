using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickUp : MonoBehaviour
{
    // Reference au joueur 
    public PlayerStats playerStats;
    // Text d'indication de la possibilité de ramasser la clé
    public GameObject ui;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Debug.Log("Le joueur à touché la clé.");
            ui.SetActive(true);
            playerStats.canTakeKey = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Debug.Log("Le joueur à touché la clé.");
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
