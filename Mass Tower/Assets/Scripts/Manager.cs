using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public TextMeshProUGUI moneyTextUI; // Assign in the inspector
    public TextMeshProUGUI scoreTextUI; // Assign in the inspector

    void Start() {
        if (CoinHandle.instance != null) {
            CoinHandle.instance.SetUIReferences(moneyTextUI, scoreTextUI);
        }
    }
}

