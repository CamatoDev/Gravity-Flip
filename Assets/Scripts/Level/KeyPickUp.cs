using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickUp : MonoBehaviour
{
    // Reference au joueur 
    PlayerStats _playerStats;
    // Text d'indication de la possibilit� de ramasser la cl�
    public GameObject ui;

    void Start()
    {
        _playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Debug.Log("Le joueur � touch� la cl�.");
            ui.SetActive(true);
            _playerStats.canTakeKey = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Debug.Log("Le joueur � touch� la cl�.");
            ui.SetActive(false);
            _playerStats.canTakeKey = false;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, range);
    //}
}
