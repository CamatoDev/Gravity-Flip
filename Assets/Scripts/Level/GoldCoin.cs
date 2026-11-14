using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GoldCoin : MonoBehaviour
{
    [Header("Paramètres Audio")]
    [Tooltip("Volume de la prise de pièce (légèrement adouci)")]
    [Range(0f, 1f)] public float volume = 0.6f;

    [Tooltip("Pitch minimal aléatoire pour varier la note sonore")]
    [Range(0.5f, 2f)] public float minPitch = 0.85f;

    [Tooltip("Pitch maximal aléatoire")]
    [Range(0.5f, 2f)] public float maxPitch = 1.25f;

    private AudioSource _audioSource;
    private Renderer[] _renderers;
    private Collider _collider;
    private bool _isCollected = false;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _renderers = GetComponentsInChildren<Renderer>();
        _collider = GetComponent<Collider>();
    }

    /// <summary>
    /// Ramassage propre : masque immédiatement le visuel de la pièce et joue le son jusqu'au bout
    /// avec une variation aléatoire du pitch.
    /// </summary>
    public void Collect()
    {
        if (_isCollected) return;
        _isCollected = true;

        // 1. Disparition visuelle et désactivation immédiate de la collision
        if (_renderers != null)
        {
            foreach (var r in _renderers)
            {
                if (r != null) r.enabled = false;
            }
        }
        if (_collider != null)
        {
            _collider.enabled = false;
        }

        // 2. Lecture intégrale du son avec pitch aléatoire et volume réduit
        if (_audioSource != null && _audioSource.clip != null)
        {
            float randomPitch = Random.Range(minPitch, maxPitch);
            _audioSource.pitch = randomPitch;
            _audioSource.volume = volume;
            _audioSource.Play();

            // Destruction différée après la fin complète du son
            float duration = _audioSource.clip.length / Mathf.Max(0.1f, Mathf.Abs(randomPitch));
            Destroy(gameObject, duration + 0.1f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }
}
