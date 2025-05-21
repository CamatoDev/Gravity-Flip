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
    // Variable pour la clé
    [Header("For the Key")]
    public bool haveKey;
    public bool canTakeKey;
    public GameObject key;
    public GameObject keyImage;

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
        if(Input.GetKeyDown(KeyCode.E) || Input.GetButton("PickUp") && canTakeKey)
        {
            haveKey = true;
            Debug.Log("Clé récuper");
            keyImage.SetActive(true);
            Destroy(key.gameObject);
        }
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.CompareTag("coinGold"))
        {
            Debug.Log("Le joueur à touché la pièce d'or.");
            Destroy(collision.transform.gameObject);
            Debug.Log("+ 10 points !");
            currentMoney += 10f;
        }

        if (collision.transform.CompareTag("BonusLife"))
        {
            Debug.Log("Le joueur à touché la vie bonus");
            Destroy(collision.transform.gameObject);
            Debug.Log("+ 1 vie !");
            currentLife += 1f;
        }

        if (collision.transform.CompareTag("BigCoinGold"))
        {
            Debug.Log("Le joueur à touché la pièce d'or géante.");
            Destroy(collision.transform.gameObject);
            Debug.Log("+ 50 points !");
            currentMoney += 50f;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Spike"))
        {
            currentLife -= spikeDamage;
        }
        if (collision.transform.CompareTag("Ground"))
        {
            isFalled = true;
        }
        if (collision.transform.CompareTag("LevelEnd"))
        {
            isLevelEnded = true;
        }
    }
}
