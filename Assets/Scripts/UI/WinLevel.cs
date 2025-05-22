using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class WinLevel : MonoBehaviour
{
    //pour le fondu 
    public SceneFader sceneFader;

    public string levelToLoad = "MainMenu";

    //pour le deverouillage du niveau suivant
    public string nextLevel = "level2";
    public int levelToUnlock = 2;

    //Source audio 
    public AudioSource buttons;

    public GameObject NextButton;


    // Start is called before the first frame update
    void Start()
    {
        //Passer un bouton en priorité à l'Event system
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(NextButton);
    }
    //void OnEnable()
    //{

    //    //if (levelToUnlock > PlayerPrefs.GetInt("levelReached", 1))
    //    //{
    //    //    PlayerPrefs.SetInt("levelReached", levelToUnlock);
    //    //}
    //}

    //Pour passer au niveau suivant 
    public void NextLevel()
    {
        //On joue le son au click sur le bouton
        buttons.Play();
        sceneFader.FadeTo(nextLevel);
    }

    //Pour réessayer 
    public void Retry()
    {
        //On joue le son au click sur le bouton
        buttons.Play();
        sceneFader.FadeTo(SceneManager.GetActiveScene().name);
    }

    //Pour revenir au menu principal 
    public void Menu()
    {
        //On joue le son au click sur le bouton
        buttons.Play();
        sceneFader.FadeTo(levelToLoad);
    }
}
