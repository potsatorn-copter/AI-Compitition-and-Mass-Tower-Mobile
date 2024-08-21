using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    public static CountdownTimer Instance { get; private set; }

    [SerializeField] private float countdownTime = 10f; // ระยะเวลาสำหรับนับถอยหลัง
    [SerializeField] private TextMeshProUGUI countdownText; // TextMeshProUGUI สำหรับแสดงเวลา

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // ทำลายอ็อบเจ็กต์ที่ซ้ำ
        }
    }

    private void Start()
    {
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        float timeRemaining = countdownTime;

        while (timeRemaining > 0)
        {
            countdownText.text = Mathf.Round(timeRemaining).ToString(); // อัปเดตข้อความของ TextMeshProUGUI
            timeRemaining -= Time.deltaTime;
            yield return null;
        }

        countdownText.text = "Enemy Invade!!"; // แสดงข้อความเมื่อนับถอยหลังเสร็จ
    }
}
