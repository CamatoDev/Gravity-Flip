using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    //Reférence à scene fader 
    public SceneFader sceneFader;

    // Vaiables pour le text du UI de Game Over
    public Text gameOverText;
    public string deadText;
    public string endTimeText;
    //Source audio 
    public AudioSource buttons;

    // Update is called once per frame
    void Update()
    {
        //if (GameManager.gameIsOver)
        //{
        //    return;
        //}

        //if (Player_stats.isDead)
        //{
        //    gameOverText.text = deadText;
        //    GameOverf();
        //}

        ////Si le temps imparti est terminé
        //if (Player_stats.playerTime <= 0f)
        //{
        //    gameOverText.text = endTimeText;
        //    GameOverf();
        //}
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
