using System.Collections;
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
    
    [Header("Objet à enclencher")]
    public GameObject target;

    [Header("Son de la plaque")]
    public AudioSource audioSource;
    public AudioClip clickClip;

    [Header("Disparition après activation")]
    [Tooltip("Délai en secondes avant de faire disparaître la plaque après activation")]
    public float disappearDelay = 0.2f;

    private bool _isActivated = false;
    private bool isPlayerInRange = false;

    void Update()
    {
        if (_isActivated) return;

        if (activationType == ActivationType.Manual && isPlayerInRange && Input.GetKeyDown(activationKey))
        {
            ActivatePlate();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isActivated) return;
        if (!collision.transform.CompareTag("Player")) return;

        if (activationType == ActivationType.Automatic)
        {
            ActivatePlate();
        }
        else // Manual
        {
            isPlayerInRange = true;
            if (indicationPressurePlate != null) indicationPressurePlate.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isActivated) return;
        if (!other.CompareTag("Player")) return;

        if (activationType == ActivationType.Automatic)
        {
            ActivatePlate();
        }
        else
        {
            isPlayerInRange = true;
            if (indicationPressurePlate != null) indicationPressurePlate.SetActive(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (_isActivated) return;
        if (!collision.transform.CompareTag("Player")) return;

        if (activationType == ActivationType.Manual)
        {
            isPlayerInRange = false;
            if (indicationPressurePlate != null) indicationPressurePlate.SetActive(false);
        }
    }

    public void ActivatePlate()
    {
        if (_isActivated) return;
        _isActivated = true;

        // 1. Jouer le son
        if (audioSource != null)
        {
            if (clickClip != null)
                audioSource.PlayOneShot(clickClip);
            else
                audioSource.Play();
        }

        // 2. Activer la cible (ex: le pont dynamique)
        if (target != null)
        {
            target.SetActive(true);
        }

        // 3. Masquer l'indication éventuelle
        if (indicationPressurePlate != null)
        {
            indicationPressurePlate.SetActive(false);
        }

        // 4. Lancer la disparition du parent immédiat (ex: 'Pressure Plate (1)')
        StartCoroutine(DisappearRoutine());
    }

    private IEnumerator DisappearRoutine()
    {
        // Désactive immédiatement les colliders pour éviter tout re-déclenchement physique
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // Petit délai pour laisser le son se jouer et l'enfoncement s'amorcer
        yield return new WaitForSeconds(disappearDelay);

        // Désactive le parent immédiat sans toucher au parent global (Pressure Plate and Brige)
        if (transform.parent != null && !transform.parent.name.Contains("Brige") && !transform.parent.name.Contains("Bridge"))
        {
            transform.parent.gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
