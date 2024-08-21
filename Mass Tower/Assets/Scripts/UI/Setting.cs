using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting : MonoBehaviour
{
    
    public GameObject[] buttonsToDisplay;  // ประกาศอาร์เรย์ของ GameObjects ที่คุณต้องการแสดง

    private bool areButtonsVisible = false;
    
    public void ToggleButtons()
    {
        areButtonsVisible = !areButtonsVisible; // เปลี่ยนสถานะของการแสดงปุ่ม
        
        // วนลูปเรียกเมท็อด SetActive() เพื่อแสดงหรือซ่อนปุ่ม
        foreach (GameObject button in buttonsToDisplay)
        {
            button.SetActive(areButtonsVisible);
        }
    }
}
