using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    // Reference a scene fader 
    public SceneFader sceneFader;

    public GameObject RetryButton;
    // Source audio 
    public AudioSource buttons;

    void OnEnable()
    {
        FocusMenu();
    }

    void Update()
    {
        // Empeche tout defocus : si rien n'est selectionne, refocaliser le bouton Retry
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
        {
            FocusMenu();
        }
    }

    private void FocusMenu()
    {
        if (EventSystem.current != null && RetryButton != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(RetryButton);
        }
    }

    public void Retry()
    {
        if (buttons != null) buttons.Play();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Retourner au menu 
    public void MainMenu()
    {
        if (buttons != null) buttons.Play();
        Time.timeScale = 1f;
        if (sceneFader != null)
        {
            sceneFader.FadeTo("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
