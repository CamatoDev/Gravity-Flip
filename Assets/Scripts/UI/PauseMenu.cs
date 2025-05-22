using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject continueButton;
    //pour le fondu 
    public SceneFader sceneFader;
    public string levelToLoad = "MainMenu";

    // Pour le son 
    public AudioSource audioSource;

    public GameObject pauseUI;

    private void Start()
    {
        //Passer un bouton en priorité à l'Event system
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(continueButton);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Pause"))
        {
            Toggle();
        }
    }
    //fonction pour basculer entre pause et continuer 
    public void Toggle()
    {
        pauseUI.SetActive(!pauseUI.activeSelf);

        //mettre le temps sur pause si on est en pause et le faire continuer sinon
        if (pauseUI.activeSelf)
        {
            Time.timeScale = 0f; //temps en pause 
        }
        else
        {
            Time.timeScale = 1f; //temps sur play 
        }
        audioSource.Play();
    }

    //fonction pour recommencé le niveau depuis le menu de pause 
    public void Retry()
    {
        Toggle(); //remet le temps en vitesse normal
        sceneFader.FadeTo(SceneManager.GetActiveScene().name);
    }

    //fonction pour retourner au menu principale 
    public void Menu()
    {
        Time.timeScale = 1f; //temps sur play
        audioSource.Play();
        sceneFader.FadeTo(levelToLoad);
    }
}
