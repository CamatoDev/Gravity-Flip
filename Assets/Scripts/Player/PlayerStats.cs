using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    // Variable pour l'argent du jouer 
    [Header("Player Money")]
    public float currentMoney;
    public Text MoneyTextUI;
    private float _startMoney = 0f;
    // Variable pour la vie du jouer 
    [Header("Player Life")]
    public Text lifeNumberUI;
    public float currentLife;
    private float _startLife = 3f;
    // Variable pour la cle
    [Header("For the Key")]
    public bool haveKey;
    public bool canTakeKey;
    public GameObject key;
    public GameObject keyImage;
    // Variables pour les sons
    [Header("Audio Source")]
    public AudioSource bigGoldCoin;
    public AudioSource bonusLife;
    public AudioSource pickUp;
    public AudioSource spike;
    public AudioSource pain;

    [Header("Audio Settings (Bonus Life & Big Coin)")]
    [Range(0f, 1f)] public float bonusLifeVolume = 0.65f;
    [Range(0.5f, 2f)] public float bonusLifeMinPitch = 0.9f;
    [Range(0.5f, 2f)] public float bonusLifeMaxPitch = 1.15f;
    [Range(0f, 1f)] public float bigCoinVolume = 0.7f;
    [Range(0.5f, 2f)] public float bigCoinMinPitch = 0.9f;
    [Range(0.5f, 2f)] public float bigCoinMaxPitch = 1.15f;

    [Header("Others")]
    public bool isFalled = false;
    public bool isLevelEnded = false;
    public float spikeDamage = 1f;

    // Start is called before the first frame update
    void Start()
    {
        currentLife = _startLife;
        currentMoney = _startMoney;
        haveKey = false;
        canTakeKey = false;
        keyImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        lifeNumberUI.text = currentLife.ToString();
        MoneyTextUI.text = currentMoney.ToString();

        // Securite de chute sous le niveau (Game Over immediat)
        if (transform.position.y < -4f)
        {
            isFalled = true;
        }

        if ((Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("PickUp")) && canTakeKey && key != null)
        {
            if (pickUp != null) pickUp.Play();
            haveKey = true;
            Debug.Log("Cle recuperee");
            if (keyImage != null) keyImage.SetActive(true);
            Destroy(key.gameObject, 0.2f);
        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.CompareTag("KillZone") || collision.transform.CompareTag("Ground"))
        {
            isFalled = true;
        }

        if (collision.transform.CompareTag("LevelEnd"))
        {
            isLevelEnded = true;
        }

        if (collision.transform.CompareTag("coinGold"))
        {
            GoldCoin coin = collision.GetComponent<GoldCoin>();
            if (coin != null)
            {
                coin.Collect();
            }
            else
            {
                Destroy(collision.transform.gameObject, 0.2f);
            }
            currentMoney += 10f;
        }

        if (collision.transform.CompareTag("BonusLife"))
        {
            if (bonusLife != null)
            {
                bonusLife.pitch = Random.Range(bonusLifeMinPitch, bonusLifeMaxPitch);
                bonusLife.volume = bonusLifeVolume;
                bonusLife.Play();
            }
            Renderer[] renderers = collision.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers) { if (r != null) r.enabled = false; }
            Collider col = collision.GetComponent<Collider>();
            if (col != null) col.enabled = false;
            Destroy(collision.transform.gameObject, 1.5f);
            currentLife += 1f;
        }

        if (collision.transform.CompareTag("BigCoinGold"))
        {
            if (bigGoldCoin != null)
            {
                bigGoldCoin.pitch = Random.Range(bigCoinMinPitch, bigCoinMaxPitch);
                bigGoldCoin.volume = bigCoinVolume;
                bigGoldCoin.Play();
            }
            Renderer[] renderers = collision.GetComponentsInChildren<Renderer>();
            foreach (var r in renderers) { if (r != null) r.enabled = false; }
            Collider col = collision.GetComponent<Collider>();
            if (col != null) col.enabled = false;
            Destroy(collision.transform.gameObject, 1.5f);
            currentMoney += 50f;
        }

        if (collision.transform.CompareTag("Spike"))
        {
            // Rajouter une animation de d�gat
            spike.Play();
            currentLife -= spikeDamage;
            pain.Play();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("KillZone") || collision.transform.CompareTag("Ground"))
        {
            isFalled = true;
        }
        if (collision.transform.CompareTag("LevelEnd"))
        {
            isLevelEnded = true;
        }
    }
}
