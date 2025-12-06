using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public float gameTimer;
    public bool startTimer;
    public PlayerHUD HUD;
    public GameObject endGameCanvas;
    public int Points;
    public bool startGame = true;
    public int SceneIndexToReload = 1;
   
    private void OnEnable()
    {
        CoinEvents.OnCoinCollected.AddListener(UpdateScoreUI);
        
    }
    
    public void Update()
    {
        if (startTimer)
        {
            gameTimer -= Time.deltaTime;
            int minutes = Mathf.FloorToInt(gameTimer / 60);
            int seconds = Mathf.FloorToInt(gameTimer % 60);
            HUD.SetTimerValue(minutes, seconds);
            if (gameTimer <= 0)
            {
                HUD.SetTimerValue(0, 0);
                startTimer = false;
                gameTimer = 0;
                GameEndTime("You LOSE!!!");
                return;
            }
        }
    }
    public void GameEndTime(string Text)
    {
        Time.timeScale = 0;
        EnableEndCanvas(Text);
    }


    private void UpdateScoreUI(int newScore)
    {
        Points = newScore;
        HUD.SetObjectiveText(newScore);
        if(Points >= 5)
        {
            GameEndTime("YOU WIN!!!!");
        }
    }
    public void EnableEndCanvas(string text)
    {
        endGameCanvas.SetActive(true);
        HUD.SetEndGameText(text);
    }

    private void OnDisable()
    {
        CoinEvents.OnCoinCollected.RemoveListener(UpdateScoreUI);
    }


    public void QuitBtn()
    {
        Application.Quit();
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneIndexToReload);
        Time.timeScale = 1;
    }
}

public static class CoinEvents//Event System
{
    public static readonly UnityEvent<int> OnCoinCollected = new UnityEvent<int>();
}
