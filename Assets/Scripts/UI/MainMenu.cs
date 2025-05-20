using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Variables pour le son 
    private AudioSource audioSource;
    public AudioSource buttonAudioSource;
    //pour le panel des option
    public GameObject optionPanel;

    // Variales pour le chargement du niveau
    public string levelToLoad = "Level01";
    public SceneFader sceneFader;

    // Start is called before the first frame update
    void Start()
    {
        // Assignation des variables 
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    // Fonction pour le lacement du jeu 
    public void Play()
    {
        //Lacement du jeu 
        buttonAudioSource.Play();
        sceneFader.FadeTo(levelToLoad);
    }

    // Fonction pour les réglages
    public void Help()
    {
        //
        buttonAudioSource.Play();
        Debug.Log("Ouvertures des options de personnalisations");
        sceneFader.FadeTo("Tutorial 1");
    }

    //fonction pour affiché le menu des options
    public void Option()
    {
        //lancement du menu des option (controle du volume, la qualité graphique)
        optionPanel.SetActive(true);
    }

    //fonction pour sortir du menu des option
    public void QuitOption()
    {
        optionPanel.SetActive(false);
    }

    // Fonction pour quitter le jeu 
    public void Quit()
    {
        buttonAudioSource.Play();
        Debug.Log("Fermeture du jeu...");
        Application.Quit();
    }
}
