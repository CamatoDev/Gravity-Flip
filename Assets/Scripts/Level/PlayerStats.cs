using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //Variable pour la clé
    public bool haveKey;
    public bool canTakeKey;
    public GameObject key;
    public GameObject keyImage;
    // Start is called before the first frame update
    void Start()
    {
        haveKey = false;
        canTakeKey = false;
        keyImage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && canTakeKey)
        {
            haveKey = true;
            Debug.Log("Clé récuper");
            keyImage.SetActive(true);
            Destroy(key.gameObject);
        }
    }
}
