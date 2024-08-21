using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnCollision : MonoBehaviour
{
    public  int scoreValue;
    public Text collisionCountText; // Text Element ที่รับค่าจำนวนการชน
    private int collisionCount = 0;
    public GameObject uiObject;
    public int limitCollision;

    private void Update()
    {
        UpdateCollisionCountText();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Collision with enemy");
        
            // ตรวจสอบว่า instance ของ SoundManager ไม่เป็น null ก่อนเล่นเสียง
            if (SoundManager.instance != null)
            {
                SoundManager.instance.Play(SoundManager.SoundName.Collision);
            }
            else
            {
                Debug.LogWarning("SoundManager instance is null");
            }
        
            // ตรวจสอบว่า collision.gameObject ไม่เป็น null ก่อนเรียก Coroutine
            if (collision.gameObject != null)
            {
                StartCoroutine(ChangeColorAfterCollision(collision.gameObject));
            }
            else
            {
                Debug.LogWarning("Collision game object is null");
            }
        
            scoreValue++;
            collisionCount++;

            // เมื่อการชนถึง 5 ครั้ง
            if (collisionCount >= limitCollision)
            {
                // ตรวจสอบว่า uiObject ไม่เป็น null ก่อนการทำให้มัน active
                if (uiObject != null)
                {
                    Time.timeScale = 0;
                    uiObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("UI object is null");
                }

                // ตรวจสอบว่า instance ของ CoinHandle ไม่เป็น null ก่อน reset ค่าเงิน
                if (CoinHandle.instance != null)
                {
                    CoinHandle.instance.ResetCurrencyOnGameOver();
                }
                else
                {
                    Debug.LogWarning("CoinHandle instance is null");
                }
            }
        }
    }
    private void UpdateCollisionCountText()
    {
        if (collisionCountText != null)
        {
            collisionCountText.text = scoreValue.ToString(); // อัปเดต Text Element ด้วยค่าจำนวนการชน
        }
    }
    

    private IEnumerator ChangeColorAfterCollision(GameObject obj)
    {
        Color originalColor = this.gameObject.GetComponent<SpriteRenderer>().material.color;
        Color redColor = Color.red;
        float startTime = Time.time;

        while (Time.time - startTime < 0.3f) 
        {
            float t = (Time.time - startTime) / 0.3f; 
            this.gameObject.GetComponent<SpriteRenderer>().material.color = Color.Lerp(originalColor, redColor, t);

            yield return null;
        }

        this.gameObject.GetComponent<SpriteRenderer>().material.color = originalColor;
    }
    
}

