using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Services.Analytics;
using UnityEngine.UI;

public class CoinHandle : MonoBehaviour
{
    public static CoinHandle instance;
    private int money;
    private int score; // ตัวแปรสำหรับเก็บสกอร์
   [SerializeField] private TextMeshProUGUI moneyText; // Reference to the TextMeshProUGUI element
   [SerializeField] private TextMeshProUGUI scoreText; // Reference to the TextMeshProUGUI element for score
    


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // เพิ่มบรรทัดนี้
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

    }
    

    public void EarnMoney(int amount)
    {
        money += amount;
        UpdateMoneyUI();
    }

    public void EarnScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    public bool SpendMoney(int amount)
    {
        if (amount <= money)
        {
            money -= amount;
            UpdateMoneyUI();
            return true;
        }
        else
        {
            return false;
        }
    }
    

    private void UpdateMoneyUI()
    {
        // Update the TextMeshProUGUI text to reflect the new money amount
        moneyText.text = $"{money}";
    }

    private void UpdateScoreUI()
    {
        scoreText.text = $"{score}";
    }
    public int GetMoney()
    {
        return money;
    }

    public int GetScore()
    {
        return score;
    }
    public void SetUIReferences(TextMeshProUGUI moneyTextUI, TextMeshProUGUI scoreTextUI) {
        moneyText = moneyTextUI;
        scoreText = scoreTextUI;
        UpdateMoneyUI();
        UpdateScoreUI();
    }
    public void ResetCurrencyOnGameOver()
    {
        money = 0;
        score = 0;
        UpdateMoneyUI();
        UpdateScoreUI();
    }
}

