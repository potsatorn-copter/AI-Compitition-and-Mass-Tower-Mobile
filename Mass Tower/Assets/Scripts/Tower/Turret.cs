using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Turret : MonoBehaviour
{[SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;
    [Header("Attribute")]
    [SerializeField] private float targetingRange = 5f;
    [SerializeField] private float bps = 1f; // BPS == Bullet per sec

    private Transform target;
    private float timeUntilFire;

    private void Start()
    {
        // Initialize the bullet pool with a certain number of bullets.
        BulletPool.instance.InitializePool(20); // For example, initializing with 10 bullets.
    }
    private void Update()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        if (!CheckTargetIsInRange()) // Check range
        {
            target = null;
            return;
        }

        timeUntilFire += Time.deltaTime;

        if (timeUntilFire >= 1f / bps)
        {
            Shoot();
            timeUntilFire = 0f;
        }
    }

    private void Shoot()
    {
        GameObject bulletObj = BulletPool.instance.GetBullet();
        if (bulletObj == null)
        {
            Debug.LogError("GetBullet() returned null. Is your BulletPool set up correctly?");
            return;
        }

        bulletObj.transform.position = firingPoint.position;
        bulletObj.transform.rotation = Quaternion.identity;
        bulletObj.SetActive(true);

        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetTarget(target);
            bulletScript.SetReturnToPoolAction(() => BulletPool.instance.ReturnBullet(bulletObj));
        }
        else
        {
            Debug.LogError("Bullet script not found on the bullet object. Is your bullet prefab set up correctly?");
            return;
        }

        // Check if SoundManager instance is not null before playing sound
        if (SoundManager.instance != null)
        {
            SoundManager.instance.Play(SoundManager.SoundName.Shoot);
        }
        else
        {
            Debug.LogWarning("SoundManager instance is null");
        }
    }

    private void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, Vector2.zero, 0f, enemyMask);
        if (hits.Length > 0)
        {
            target = hits[0].transform;
        }
    }

    private bool CheckTargetIsInRange()
    {
        // Ensure target is not null before checking range
        if (target != null)
        {
            return Vector2.Distance(target.position, transform.position) <= targetingRange;
        }
        return false;
    }
}





