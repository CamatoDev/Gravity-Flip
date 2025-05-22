using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    //Reférence à scene fader 
    public SceneFader sceneFader;

    public GameObject RetryButton;
    //Source audio 
    public AudioSource buttons;

    void Start()
    {
        //Passer un bouton en priorité à l'Event system
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(RetryButton);
    }

    public void Retry()
    {
        //On joue le son au click sur le bouton
        buttons.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Retourner au menu 
    public void MainMenu()
    {
        //On joue le son au click sur le bouton
        buttons.Play();
        sceneFader.FadeTo("MainMenu");
    }
}
