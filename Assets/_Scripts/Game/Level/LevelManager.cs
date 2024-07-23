using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager manager;
    [SerializeField]
    private float timer = 30.00f;
    [SerializeField]
    private int killingTarget = 10;
    int killingCounter = 0;

    public bool gameOver = false;

    // Panels
    public GameObject gameOverPanel;
    public GameObject gameCompletePanel;

    // Game Texts
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI killingText;
    public TextMeshProUGUI killingResultText;

    // Timer variables
    float minute;
    float seconds;

    private void Awake() 
    {
        UnpauseGame();

        if(manager == null)
        {
            manager = this;
        }

        gameOverPanel.SetActive(false);
        gameCompletePanel.SetActive(false);

        killingText.text = "0/" + killingTarget;
    }

    private void Update() 
    {
        if(PlayerManager.instance.floor.activeSelf)
        {
            // Timer Countdown
            if(timer >= 0 && !gameOver && killingCounter < killingTarget)
            {
                timer -= Time.deltaTime;
                minute = Mathf.FloorToInt(timer / 60);
                seconds = Mathf.FloorToInt(timer % 60);

                // Update timer text
                // timerText.text = "00:" + timer.ToString("F0");
                timerText.text = minute.ToString() + ":" + seconds.ToString();
            }
            else if (timer < 0 || gameOver)
            {
                gameOver = true;
                // PauseGame();
                PlayerManager.instance.KillPlayer();
                InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
                ToggleGamePanel(gameOverPanel);
            }

            if(!gameOver && killingCounter >= killingTarget)
            {
                // PauseGame();
                PlayerManager.instance.KillPlayer();
                killingResultText.text = "Your time: " + minute.ToString() + ":" + seconds.ToString();
                ToggleGamePanel(gameCompletePanel);
            }
        }
    }

    public void ToggleGamePanel(GameObject panel)
    {
        if(panel.activeSelf != true)
            panel.SetActive(true);
    }

    public void UpdateKillings()
    {
        killingCounter += 1;
        killingText.text = killingCounter.ToString() + "/" + killingTarget;
    }

    public void RestartGame()
    {
        Debug.Log("Game Restarted!");
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SceneManager.LoadScene(0);
    }

    public void LevelComplete()
    {
        int levelIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log(SceneManager.sceneCountInBuildSettings);
        if(levelIndex + 1 < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(levelIndex + 1);
        else
            SceneManager.LoadScene(0);
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0000001f;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
    }
}
