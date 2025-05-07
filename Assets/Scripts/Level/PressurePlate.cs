using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PressurePlate : MonoBehaviour
{
    public enum ActivationType { Automatic, Manual }
    [Header("Type de plaque")]
    public ActivationType activationType = ActivationType.Automatic;

    [Header("Manuel uniquement")]
    public KeyCode activationKey = KeyCode.E;

    [Header("Indication à (dé)activer")]
    public GameObject indicationPressurePlate;
    
    [Header("objet à enclencher")]
    public GameObject target;

    // état interne
    private bool isPlayerInRange = false;

    void Reset()
    {
        // assure qu'on peut passer à travers la plaque
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void Update()
    {
        if (activationType == ActivationType.Manual && isPlayerInRange && Input.GetKeyDown(activationKey))
        {
            TogglePlate();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (activationType == ActivationType.Automatic)
        {
            //gameObject.SetActive(false);
            TriggerAction();
        }
        else // Manual
        {
            isPlayerInRange = true;
            // ici tu peux afficher un prompt UI : "Appuyez sur E"
            indicationPressurePlate.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (activationType == ActivationType.Automatic)
        {
            //gameObject.SetActive(true);
        }
        else // Manual
        {
            isPlayerInRange = false;
            // masquer le prompt UI
            indicationPressurePlate.SetActive(false);
        }
    }
    public void TogglePlate()
    {
        //gameObject.SetActive(!gameObject.activeSelf);
        indicationPressurePlate.SetActive(!indicationPressurePlate.activeSelf);
        target.SetActive(true);
    }

    public void TriggerAction()
    {

    }
}

