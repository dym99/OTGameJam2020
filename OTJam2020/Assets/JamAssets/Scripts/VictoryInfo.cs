using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VictoryInfo : MonoBehaviour
{
    public TMP_Text m_text;

    public void OnEnable() {
        m_text.text = $"Level Score: {LevelManager.currentLevel.levelScore}\nTotal Score: {GameManager.instance.totalScore}";
    }
}
