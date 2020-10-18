using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Init() {
        GameObject go = new GameObject("[Game Manager]");
        instance = go.AddComponent<GameManager>();
        DontDestroyOnLoad(go);
    }

    public void NewGame() {
        totalScore = 0;
    }


    public int totalScore = 0;
}
