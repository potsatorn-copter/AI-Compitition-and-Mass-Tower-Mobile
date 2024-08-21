using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Platform : MonoBehaviour
{
    public GameObject towerPrefab; // Assign your Tower prefab in the inspector
    private int towerSpawned = 0; // Control variable to track the number of spawned towers
    public int maxTowers = 3; // Maximum number of towers allowed
    private bool canSpawn = true; // Additional control variable to track if we can spawn towers

    void Update()
    {
        // Check for mouse input and if we can still spawn towers
        if (canSpawn && Input.GetMouseButtonDown(0))
        {
            SpawnAtInputPosition(Input.mousePosition);
        }

        // Check for touch input on mobile devices and if we can still spawn towers
        if (canSpawn && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            SpawnAtInputPosition(touch.position);
        }
    }

    void SpawnAtInputPosition(Vector3 inputPosition)
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(inputPosition);
        worldPosition.z = 0; // Set Z to 0 for 2D gameplay

        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero);
        if (hit.collider != null && hit.collider.CompareTag("TowerPlaceable"))
        {
            SpawnTowerAtPosition(hit.point);
            hit.collider.enabled = false;
            towerSpawned++; // Increment the number of towers spawned

            // Disable the collider to prevent further towers from being spawned here
          

            // If max towers have been spawned, stop spawning and destroy all tower placeable areas
            if (towerSpawned >= maxTowers)
            {
                canSpawn = false; // No longer allow spawning
                DestroyAllTowerPlaceableAreas();
            }
        }
    }

    void SpawnTowerAtPosition(Vector2 position)
    {
        Instantiate(towerPrefab, position, Quaternion.identity); // Instantiate the tower at the given position
        
    }

    void DestroyAllTowerPlaceableAreas()
    {
        GameObject[] towerPlaceableAreas = GameObject.FindGameObjectsWithTag("TowerPlaceable");
        foreach (GameObject area in towerPlaceableAreas)
        {
            Destroy(area); // Destroy the placeable area
        }
    }
}

