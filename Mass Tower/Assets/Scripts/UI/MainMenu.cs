using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        Time.timeScale = 1; // Reset timeScale to normal
        SceneManager.LoadScene("Consent");
    }

    public void QuitGame()
    { 
        // Quit Game doesn't need to change timeScale since the game is closing
        Application.Quit();
    }

    public void Credits()
    { 
        Time.timeScale = 1; // Reset timeScale to normal
        SceneManager.LoadScene("Credits");
    }

    public void Back()
    {
        Time.timeScale = 1; // Reset timeScale to normal
        SceneManager.LoadScene("Main Menu");
    }

    public void Next()
    {
        Time.timeScale = 1; // Reset timeScale to normal
        SceneManager.LoadScene("Shop");
    }

    public void Next2()
    {
        Time.timeScale = 1; // Reset timeScale to normal
        SceneManager.LoadScene("Shop1");
    }

    public void Restart()
    {
        Time.timeScale = 1; // Reset timeScale to normal
        SceneManager.LoadScene("GamePlay2");
    }
    public void Restarts()
    {
        Time.timeScale = 1; // Reset timeScale to normal
        SceneManager.LoadScene("GamePlay");
    }
    public void GamePlay3()
    {
        SceneManager.LoadScene("GamePlay3");
    }

    public void GamePlay2()
    {
        SceneManager.LoadScene("GamePlay2");
    }
}
