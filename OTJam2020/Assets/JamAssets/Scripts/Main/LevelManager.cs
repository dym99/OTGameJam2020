using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager currentLevel { get; private set; }

    public GameObject m_pauseMenu;
    public GameObject m_victoryPanel;

    static string menuLevel = "MainMenu";


    private void Start() {
        LevelManager.currentLevel = this;
        __levelScore = 0;
        Time.timeScale = 1.0f;
    }

    private int __levelScore;
    public int levelScore {
        get {
            return __levelScore;
        }
    }
    public float levelTime;
    public string nextLevel;

    public void ScorePoints(int points) {
        __levelScore += points;
    }

    public void NextLevel() {
        SceneManager.LoadScene(nextLevel);
    }


    public void Pause() {
        Time.timeScale = 0.0f;
        m_pauseMenu.SetActive(true);
    }

    public void Unpause() {
        m_pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void ResetLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToMenu() {
        SceneManager.LoadScene(menuLevel);
    }

    public void Victory() {
        GameManager.instance.totalScore += levelScore;
        m_victoryPanel.SetActive(true);
    }
}
