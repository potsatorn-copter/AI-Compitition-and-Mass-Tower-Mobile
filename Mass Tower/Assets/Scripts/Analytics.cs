using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Analytics : MonoBehaviour
{
    public GameObject consentPanel; // อ้างอิงไปยังปุ่ม UI ของคุณ
    public Button acceptButton; // ปุ่มยอมรับ
    public Button declineButton; // ปุ่มปฏิเสธ

    // Start is called before the first frame update
    async void Start()
    {
        // ตรวจสอบว่า Unity Services ได้เริ่มต้นหรือยัง
        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
        {
            // ถ้ายังไม่ได้เริ่มต้น ทำการเริ่มต้น
            await UnityServices.InitializeAsync();
        }

        // แสดง Consent Panel ทุกครั้งที่เริ่มฉาก
        consentPanel.SetActive(true);
    }

    void OnEnable()
    {
        // เพิ่ม Listener ให้กับปุ่ม
        acceptButton.onClick.AddListener(ConsentGiven);
        declineButton.onClick.AddListener(ConsentDenied);
    }

    void OnDisable()
    {
        // ลบ Listener ออกเมื่อสคริปต์ไม่ถูกใช้งาน
        acceptButton.onClick.RemoveListener(ConsentGiven);
        declineButton.onClick.RemoveListener(ConsentDenied);
    }

    void ConsentGiven()
    {
        // เริ่มการเก็บข้อมูล Analytics
        AnalyticsService.Instance.StartDataCollection();
        consentPanel.SetActive(false);
        SceneManager.LoadScene("GamePlay");
    }

    void ConsentDenied()
    {
        // จัดการกรณีที่ผู้เล่นปฏิเสธ
        consentPanel.SetActive(false);
        // ที่นี่คุณอาจจะแสดงข้อความว่าไม่เก็บข้อมูลผู้เล่นหรือปิดการใช้งานฟีเจอร์อื่นๆ
        SceneManager.LoadScene("GamePlay");
    }
}
