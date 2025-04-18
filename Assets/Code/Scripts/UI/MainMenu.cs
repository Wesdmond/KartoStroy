using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // [SerializeField] private string GameSceneName = "";

    private void Start()
    {
        // if (GameSceneName == "")
        //     Debug.LogError("No game scene name");
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        // SceneManager.LoadScene(SceneManager.GetSceneByName(GameSceneName).buildIndex);
    }

    public void ExitGame()
    {
        Debug.Log("Exit Game");
        Application.Quit();
    }
}