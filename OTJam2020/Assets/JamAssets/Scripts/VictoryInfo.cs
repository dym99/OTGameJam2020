using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VictoryInfo : MonoBehaviour
{
    public TMP_Text m_text;

    public void OnEnable() {
        m_text.text = $"Time Bonus: {LevelManager.currentLevel.m_timeBonus}\nLevel Score: {LevelManager.currentLevel.levelScore}\nTotal Score: {GameManager.instance.totalScore}";
    }
}
