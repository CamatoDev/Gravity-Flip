using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //Variable pour la cl�
    private bool _haveKey;
    public bool canTakeKey;
    public GameObject key;
    // Start is called before the first frame update
    void Start()
    {
        _haveKey = false;
        canTakeKey = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && canTakeKey)
        {
            _haveKey = true;
            Debug.Log("Cl� r�cuper");
            Destroy(key.gameObject);
        }
    }
}
