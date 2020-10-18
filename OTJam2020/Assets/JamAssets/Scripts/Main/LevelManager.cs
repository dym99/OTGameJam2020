using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.ComponentModel;

public class LevelManager : MonoBehaviour
{
    public static LevelManager currentLevel { get; private set; }

    public GameObject m_pauseMenu;
    public GameObject m_victoryPanel;

    public TMP_Text m_timerText;

    static string menuLevel = "MainMenu";

    public float m_levelFastest = 3.0f;
    public float m_levelPar = 5.0f;
    public float m_levelTime = 0.0f;
    public int m_maxTimeBonus = 1000;

    public int m_timeBonus;

    private bool m_finished;

    private void Start() {
        LevelManager.currentLevel = this;
        __levelScore = 0;
        Time.timeScale = 1.0f;
        m_finished = false;
    }

    private void FixedUpdate() {
        if (!m_finished)
            m_levelTime += Time.fixedDeltaTime;
        int minutes = (int)m_levelTime / 60;
        int seconds = (int)m_levelTime % 60;
        string padding0 = "";
        if (seconds < 10) {
            padding0 = "0";
        }
        m_timerText.text = $"Time: {minutes}:{padding0}{seconds}";
    }

    private int __levelScore;
    public int levelScore {
        get {
            return __levelScore;
        }
    }
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
        float lowerBound = m_levelFastest;
        float upperBound = m_levelPar + (m_levelPar - m_levelFastest);
        m_timeBonus = (int)(m_maxTimeBonus * Mathf.InverseLerp(upperBound, lowerBound, Mathf.Clamp(m_levelTime, lowerBound, upperBound)));
        ScorePoints(m_timeBonus);
        GameManager.instance.totalScore += levelScore;
        m_victoryPanel.SetActive(true);
        m_finished = true;
    }
}
