using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Time")]
    public float timer = 300f;
    public Text timerText;
    [Header("Game is over")]
    private bool _gameIsOver;
    public GameObject gameOver;
    // Start is called before the first frame update
    void Start()
    {
        _gameIsOver = false;
        gameOver.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        timer = Mathf.Clamp(timer, 0f, Mathf.Infinity);

        timerText.text = "Timer : " + string.Format("{0:00.}", timer);

        if(timer <= 0)
        {
            GameOver();
        }
    }

    // Fonction pour la défaite 
    public void GameOver()
    {
        _gameIsOver = true;
        gameOver.SetActive(true);
    }
}
