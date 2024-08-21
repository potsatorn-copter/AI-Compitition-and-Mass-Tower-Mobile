using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Bullet : MonoBehaviour
{
    private Transform target;
    [Header("Reference")] [SerializeField] private Rigidbody2D rb;

    [Header("Attributes")] [SerializeField]
    private float bulletSpeed = 5f;

    [SerializeField] private BulletDamageData bulletData;

    private Action onBulletReturn;

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    public void SetReturnToPoolAction(Action returnAction)
    {
        onBulletReturn = returnAction;
    }

    private void FixedUpdate()
    {
        if (!target)
        {
            onBulletReturn?.Invoke();
            gameObject.SetActive(false);
            return;
        }

        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * bulletSpeed;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        onBulletReturn?.Invoke();
        gameObject.SetActive(false);

        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemymovement enemymovement = other.gameObject.GetComponent<Enemymovement>();
            if (enemymovement != null)
            {
                enemymovement.HitByTower(bulletData.BulletDamage);
            }
            else
            {
                Debug.LogError("EnemyMovement script not found on the enemy object. Is your enemy set up correctly?");
            }
        }

        ReturnToPool(); // Use object pooling method to return the bullet
    }

    public GameObject GetBullet()
    {
        // โค้ดสำหรับเรียก GameObject ที่เป็นกระสุน
        // ...
        return null; // ต้องเปลี่ยนโค้ดส่วนนี้ให้คืนค่า GameObject ที่ถูกต้อง
    }

// Call this function when the bullet gets out of the camera view or when it's no longer needed.
    public void ReturnToPool()
    {
        if (BulletPool.instance != null)
        {
            BulletPool.instance.ReturnBullet(gameObject);
        }
        else
        {
            Debug.LogError("BulletPool instance is null.");
            Destroy(gameObject); // Destroy the bullet if the pool is not available.
        }
    }
}
