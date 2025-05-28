using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class EphemeralPlatform : MonoBehaviour
{
    [Header("Délais")]
    [Tooltip("Temps avant disparition après contact")]
    public float disappearDelay = 0.5f;
    [Tooltip("Temps avant réapparition")]
    public float respawnDelay = 2f;

    [Header("Options")]
    [Tooltip("Si vrai, la plateforme ne réapparaît qu'une seule fois")]
    public bool oneTime = false;

    [Header("Son de la plaque")]
    public AudioSource audioSource;

    private Collider _collider;
    private MeshRenderer _renderer;
    private bool _isCycling = false;

    void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = false;        // collision normale
        _renderer = GetComponent<MeshRenderer>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_isCycling) return;
        if (collision.collider.CompareTag("Player"))
        {
            StartCoroutine(CyclePlatform());
        }
    }

    private IEnumerator CyclePlatform()
    {
        _isCycling = true;
        yield return new WaitForSeconds(disappearDelay);

        // Désactivation visuelle et physique
        audioSource.Play();
        _renderer.enabled = false;
        _collider.enabled = false;

        if (oneTime) yield break;

        yield return new WaitForSeconds(respawnDelay);

        // Remise en place
        _renderer.enabled = true;
        _collider.enabled = true;
        _isCycling = false;
    }
}

