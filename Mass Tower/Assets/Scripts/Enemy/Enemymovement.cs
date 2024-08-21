using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.RemoteConfig;


public class Enemymovement : MonoBehaviour
{
    [SerializeField] private EnemyData moveSpeedTable;
    [SerializeField] private EnemyData maxHealth;
    public bool KilledByTower { get; private set; } = false;
    private int currentHealth;
    public Action OnDeath;
    private bool isDead = false;
   

    private Transform[] path;
    private float moveSpeed;
    private int pathIndex = 0;
    private Transform target;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public int moneyValue = 20;
    public int scoreValue = 10;

    private void Awake()
    {

        currentHealth = (int) maxHealth.Health;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Set the moveSpeed from the ScriptableObject
        if (moveSpeedTable != null)
        {
            moveSpeed = moveSpeedTable.MoveSpeed;
        }
        else
        {
            moveSpeed = 3f; // Default value if ScriptableObject is not set
        }

        // Initialize the first target if path is not null and has at least one waypoint
        if (path != null && path.Length > 0)
        {
            target = path[pathIndex];
        }
    }
    private void FixedUpdate()
    {
        if (currentHealth <= 0) return; // If the enemy is dead, don't proceed with movement

        if (pathIndex < path.Length)
        {
            MoveToPath();
        }
        else
        {
            // Optional: Do something if the enemy reaches the end of the path
        }
    }

    public void Initialize(Transform[] newPath)
    {
        path = newPath;
        pathIndex = 0;
        if (path != null && path.Length > 0)
        {
            target = path[pathIndex];
        }
    }

    private void Update()
    {
        if (pathIndex >= path.Length)
        {
            Die();
            return;
        }

        target = path[pathIndex];
        MoveToPath();
    }

    private void MoveToPath()
    {
        if (path == null || target == null)
        {
            return;
        }
        Vector2 targetPosition = path[pathIndex].position;
        Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        if (Vector2.Distance(rb.position, targetPosition) < 0.1f)
        {
            pathIndex++;
        }
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true; // ตั้งค่า isDead ก่อนเรียก Die เพื่อป้องกันการเรียกซ้ำ
            Die();
        }
        else
        {
            if (spriteRenderer != null)
            {
                StartCoroutine(FlashRed());
            }
        }
        
    }
    
    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = Color.white;
    }
   

    private void Die()
    {
        if (isDead)
        {
            if (KilledByTower)
            {
                CoinHandle.instance.EarnMoney(moneyValue); // เพิ่มเงิน
                CoinHandle.instance.EarnScore(scoreValue); // เพิ่มคะแนน
            }

            Debug.Log("Enemy died"); // เพิ่ม Debug Log

            // รหัสสำหรับการตาย
            OnDeath?.Invoke();
            // ตัวอย่างการทำลายตัวละครศัตรู
            Destroy(gameObject);
        }
    }
    public void HitByTower(int damage)
    {
        KilledByTower = true;
        TakeDamage(damage);
    }
    void OnDestroy() {
        if (KilledByTower && OnDeath != null) {
            OnDeath();
        }
        OnDeath = null;
        Debug.Log("Enemy OnDestroy called");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Zombie"))
        {
            DieImmediately();
        }
    }

    private void DieImmediately()
    {
        // ตัวอย่างการทำลายตัวละครศัตรูโดยไม่ต้องรอการเรียก OnDeath
        Destroy(gameObject);
    }

}
