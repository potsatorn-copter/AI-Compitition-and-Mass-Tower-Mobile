using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // สำหรับการเข้าถึงองค์ประกอบ UI
using TMPro;
using UnityEngine.SceneManagement; // เพิ่มบรรทัดนี้เพื่อใช้งาน TextMeshPro


public class ShopItem : MonoBehaviour
{
    public int cost; // Cost of the tower
    public Button buyButton; // Assign this through the inspector
    public TextMeshProUGUI moneyText; // Reference to the TextMeshProUGUI for displaying money
    

    private void Start()
    {
        // Make sure the Button component is attached to the same GameObject as this script
        // and the buyButton reference is assigned in the inspector.
        buyButton.onClick.AddListener(PurchaseItem); // Add listener for button click
        UpdateMoneyUI(); // Update the UI when the scene starts
    }

    private void PurchaseItem()
    {
        if(CoinHandle.instance != null && CoinHandle.instance.SpendMoney(cost))
        {
            Debug.Log("Item Purchased!");
            UpdateMoneyUI(); // Update the UI after purchase
            // Logic for what happens after purchase
        }
        else
        {
            Debug.Log("Not enough money!");
            // Inform the player they don't have enough money
        }
    }
    private void UpdateMoneyUI()
    {
        if(moneyText != null && CoinHandle.instance != null)
        {
            moneyText.text = CoinHandle.instance.GetMoney().ToString();
        }
    }
}
