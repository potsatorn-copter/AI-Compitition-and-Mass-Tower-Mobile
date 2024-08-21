using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool instance;

    [SerializeField]
    private GameObject bulletPrefab; // Prefab of the bullet to instantiate.
    private Queue<GameObject> bullets = new Queue<GameObject>(); // Queue to hold the pooled bullets.

    private void Awake()
    {
        // Ensure that there's only one instance of BulletPool.
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Method to prepare bullets and add them to the pool.
    public void InitializePool(int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bullets.Enqueue(bullet);
        }
    }

    // Method to get a bullet from the pool.
    public GameObject GetBullet()
    {
        if (bullets.Count > 0)
        {
            GameObject bullet = bullets.Dequeue();
            bullet.SetActive(true);
            return bullet;
        }
        else
        {
            // Optionally instantiate a new bullet if the pool is empty.
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(true);
            return bullet;
        }
    }

    // Method to return a bullet back to the pool.
    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bullets.Enqueue(bullet);
    }
}

