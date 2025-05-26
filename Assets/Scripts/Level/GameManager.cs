using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    PlayerStats _playerStats;
    [Header("Time")]
    private float timer;
    public float startTimer = 300f;
    public Text timerText;
    // Vaiables pour le text du UI de Game Over
    [Header("Game is over")]
    public Text gameOverText;
    public string deadText;
    public string endTimeText;
    public string fallingText;
    public GameObject gameOver;
    private bool _gameIsOver;
    // Vaiables pour le text du UI de Win Level
    [Header("Level Won")]
    public GameObject winLevel;
    //pour le text du nombre de pièces collecté
    public Text moneyNumber;
    public Text levelEndedTime;

    // Start is called before the first frame update
    void Start()
    {
        timer = startTimer;
        _gameIsOver = false;
        gameOver.SetActive(false);
        winLevel.SetActive(false);
        _playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_gameIsOver)
        {
            timer -= Time.deltaTime;
            timer = Mathf.Clamp(timer, 0f, Mathf.Infinity);
        }

        timerText.text = "Timer : " + string.Format("{0:00.}", timer);

        // Si le joueur n'a plus de temps
        if(timer <= 0)
        {
            gameOverText.text = endTimeText;
            GameOver();
        }

        if(_playerStats.currentLife <= 0f)
        {
            gameOverText.text = deadText;
            GameOver();
        }

        if (_playerStats.isFalled)
        {
            gameOverText.text = fallingText;
            GameOver();
        }

        if (_playerStats.isLevelEnded)
        {
            moneyNumber.text = "You scorce : " + _playerStats.currentMoney.ToString();
            WinLevel();
        }
    }

    // Fonction pour la défaite 
    public void GameOver()
    {
        _gameIsOver = true;
        gameOver.SetActive(true);
    }
    
    // Fonction pour la victoire 
    public void WinLevel()
    {
        _gameIsOver = true;
        levelEndedTime.text = "Your Time : " + string.Format("{0:00.00}", startTimer - timer);
        winLevel.SetActive(true);
    }
}
