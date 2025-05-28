using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Variales pour le chargement du niveau
    public string levelToLoad = "LevelSelector";
    public SceneFader sceneFader;

    public GameObject playButton;
    public AudioSource buttonAudioSource;

    public static string NameTag; //le pseudo du joueur 

    //pour le panel des option
    [Header("Option Panel")]
    public GameObject optionPanel;
    //pour la musique
    public AudioSource MusicAudioSource;
    public Slider MusicSlider;
    public Text VolumeMusicText;
    //pour les effets
    public AudioSource EffectAudioSource;
    public Slider EffectSlider;
    public Text VolumeEffectText;

    // Start is called before the first frame update
    void Start()
    {
        //Passer un bouton en priorité à l'Event system
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(playButton);

        VolumeMusicChange(); 
        VolumeEffectChange();
        NameTag = PlayerPrefs.GetString("nameTag", "INCONU");
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
        buttonAudioSource.Play();
        //Passer un bouton en priorité à l'Event system
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(MusicSlider.gameObject);
        //lancement du menu des option (controle du volume, la qualité graphique)
        optionPanel.SetActive(true);
    }

    //fonction pour modifier la valeur de la music
    public void VolumeMusicChange()
    {
        MusicAudioSource.volume = MusicSlider.value;
        VolumeMusicText.text = "Volume " + (MusicAudioSource.volume * 100).ToString("00") + " %";
    }

    //fonction pour modifier la valeur de la music des effets
    public void VolumeEffectChange()
    {
        EffectAudioSource.volume = EffectSlider.value;
        VolumeEffectText.text = "Effect " + (EffectAudioSource.volume * 100).ToString("00") + " %";
    }

    //fonction pour gérer l'ouverture du clavier
    public void OpenKeyboard()
    {
        
    }

    //fonction pour sortir du menu des option
    public void QuitOption()
    {
        buttonAudioSource.Play();
        //Passer un bouton en priorité à l'Event system
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(playButton);
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
