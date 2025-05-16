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

    void Update()
    {
        if (activationType == ActivationType.Manual && isPlayerInRange && Input.GetKeyDown(activationKey))
        {
            TogglePlate();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.CompareTag("Player")) return;

        if (activationType == ActivationType.Automatic)
        {
            Vector3 press = new Vector3(0, 0.04f, 0);
            transform.position -= press;
            TogglePlate();
        }
        else // Manual
        {
            isPlayerInRange = true;
            // "Appuyez sur E"
            indicationPressurePlate.SetActive(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.transform.CompareTag("Player")) return;

        if (activationType == ActivationType.Automatic)
        {
            Vector3 press = new Vector3(0, 0.04f, 0);
            transform.position += press;
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

