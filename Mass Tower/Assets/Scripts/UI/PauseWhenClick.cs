using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseWhenClick : MonoBehaviour
{
    private bool isGamePaused = false;

    public void ToggleGamePause()
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        private void PauseGame()
        {
            Time.timeScale = 0f; // หยุดเกม
            isGamePaused = true;
        }

        private void ResumeGame()
        {
            Time.timeScale = 1f; // ดำเนินเกมต่อ
            isGamePaused = false;
        }
    }

