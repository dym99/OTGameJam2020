using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Quit() {
        Application.Quit();
    }

    public void StartGame(string level) {
        SceneManager.LoadScene(level);
        GameManager.instance.NewGame();
    }
}
