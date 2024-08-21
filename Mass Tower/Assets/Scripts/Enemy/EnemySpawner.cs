
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform[] enemyPaths;
    [SerializeField] private int enemiesPerWave = 20; // Set to 20 enemies per wave
    [SerializeField] private float timeBeforeStartingFirstWave = 10f; // Set to wait 10 seconds before starting the wave
    private bool allEnemiesCleared = false;
    
    
    private Queue<GameObject> enemyPool = new Queue<GameObject>();
    private int enemiesAlive;
    
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        ResetGame(); // ใช้ ResetGame เพื่อตั้งค่าเริ่มต้น
        InitializeEnemyPool();
        StartCoroutine(SpawnWaves());
    }
   

    private void InitializeEnemyPool()
    {
        for (int i = 0; i < enemiesPerWave; i++) // Initialize enough enemies for the wave
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            newEnemy.SetActive(false);
            enemyPool.Enqueue(newEnemy);
        }
    }

    private IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(timeBeforeStartingFirstWave); // Wait before starting the wave
        enemiesAlive = enemiesPerWave;

        for (int i = 0; i < enemiesPerWave; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(1f); // Delay between spawns
        }

        // Removed WaitUntil as it's no longer needed
    }

    private void SpawnEnemy()
    {
        {
            if (enemyPool.Count > 0) {
                GameObject enemy = enemyPool.Dequeue();
                if (enemy != null) {
                    enemy.SetActive(true);
                    Enemymovement enemyMovement = enemy.GetComponent<Enemymovement>();
                    if (enemyMovement != null) {
                        enemyMovement.Initialize(enemyPaths);
                        enemyMovement.OnDeath += EnemyDied;
                    }
                }
            }
        }

    }
    private void OnAllEnemiesCleared()
    {
        if (allEnemiesCleared && UIManager.Instance != null)
        {
            StartCoroutine(WaitAndInvokeUI()); // Use coroutine to delay UI display.
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetGame();
    }

    private void EnemyDied()
    {
        // Decrease the count of enemies alive.
        enemiesAlive--;

        // Check if we have a UIManager instance and that all enemies are cleared.
        if (enemiesAlive <= 0 && !allEnemiesCleared)
        {
            allEnemiesCleared = true;
            OnAllEnemiesCleared(); // Call the method directly if you're not delaying UI display.
            // Instead of immediately invoking the method, wait for a short delay to ensure all deaths are processed.
        }
    }

    private void ResetGame()
    {
        Time.timeScale = 1; // ตั้งค่าเป็นปกติก่อนเริ่มเกมใหม่
        allEnemiesCleared = false;
        enemiesAlive = 0;
        // ทำการรีเซ็ตหรือทำลายอ็อบเจ็กต์ที่เหลืออยู่ใน enemyPool
        while (enemyPool.Count > 0)
        {
            GameObject enemy = enemyPool.Dequeue();
            if (enemy != null) Destroy(enemy);
        }

        // จากนั้นเติม enemyPool ใหม่
        InitializeEnemyPool();

    }
    private IEnumerator WaitAndInvokeUI()
    {
        yield return new WaitForSeconds(12f);

        // Now it's safe to check for UIManager instance and invoke the method to display UI.
        if (UIManager.Instance != null)
        {
            Time.timeScale = 0;
            UIManager.Instance.DisplayEndGameUI(true);
            Debug.Log("All enemies cleared, displaying end game UI.");
        }
    }

}





    

    


    
    

