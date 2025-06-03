using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tower_Cannon : Tower
{
    public int damage;
    public GameObject prefab_shootItem;
    public float interval;
    public Transform shootPoint;

    private List<Enemy> enemiesInRange = new List<Enemy>();
    private bool isShooting = false;
    private Animator cannonAnimator;
    private Enemy currentTargetForAnimation;

    protected override void Start()
    {
        base.Start();
        cannonAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isShooting && enemiesInRange.Count > 0)
        {
            RemoveNullEnemies(); // Bersihkan dulu list dari musuh yang sudah null/mati
            if (enemiesInRange.Count > 0) // Cek lagi setelah dibersihkan
            {
                StartCoroutine(ShootDelay());
            }
        }
    }

    IEnumerator ShootDelay()
    {
        isShooting = true;
        // Debug.Log($"ShootDelay started. Enemies: {enemiesInRange.Count}");
        while (enemiesInRange.Count > 0)
        {
            PrepareToShoot();
            yield return new WaitForSeconds(interval);
            RemoveNullEnemies(); // Penting untuk membersihkan list secara berkala
            // Debug.Log($"ShootDelay loop. Enemies after removal: {enemiesInRange.Count}");
            if (enemiesInRange.Count == 0)
            {
                // Debug.Log("ShootDelay loop broken. No enemies.");
                break;
            }
        }
        isShooting = false;
        // Debug.Log("ShootDelay ended. isShooting set to false.");
    }

    void PrepareToShoot()
    {
        if (enemiesInRange.Count == 0)
        {
            // Debug.Log("PrepareToShoot: No enemies in range.");
            currentTargetForAnimation = null;
            return;
        }

        Enemy target = GetNearestEnemy(); // Sekarang GetNearestEnemy tidak mengubah list

        if (target != null && target.gameObject.activeInHierarchy)
        {
            // Debug.Log($"PrepareToShoot: Target found - {target.name}");
            currentTargetForAnimation = target;

            if (cannonAnimator != null)
            {
                cannonAnimator.SetTrigger("Shoot");
            }
        }
        else
        {
            // Debug.LogWarning("PrepareToShoot: GetNearestEnemy returned null or inactive target.");
            currentTargetForAnimation = null;
        }
    }

    public void FireProjectileFromAnimationEvent()
    {
        if (currentTargetForAnimation == null || !currentTargetForAnimation.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("FireProjectileFromAnimationEvent: Target for animation event is null or inactive. Projectile not fired.");
            return;
        }

        // Debug.Log($"FireProjectileFromAnimationEvent: Firing at {currentTargetForAnimation.name}");
        Vector3 projectileSpawnPosition = (shootPoint != null) ? shootPoint.position : transform.position;
        Vector3 directionToTarget = (currentTargetForAnimation.transform.position - projectileSpawnPosition).normalized;
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        Quaternion projectileRotation = Quaternion.Euler(0f, 0f, angle);

        GameObject shotItem = Instantiate(prefab_shootItem, projectileSpawnPosition, projectileRotation);
        ShootItem projectileScript = shotItem.GetComponent<ShootItem>();

        if (projectileScript != null)
        {
            projectileScript.Init(damage, currentTargetForAnimation);
        }
    }

    void RemoveNullEnemies()
    {
        // Menghapus semua musuh yang null atau tidak aktif dari list
        enemiesInRange.RemoveAll(enemy => enemy == null || !enemy.gameObject.activeInHierarchy);
    }

    // VERSI GetNearestEnemy YANG TIDAK MENGUBAH LIST enemiesInRange
    Enemy GetNearestEnemy()
    {
        Enemy nearest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Enemy enemy in enemiesInRange) // Hanya iterasi, tidak ada RemoveAt(i)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy)
            {
                continue;
            }

            float distanceToEnemy = Vector3.Distance(enemy.transform.position, currentPosition);
            if (distanceToEnemy < minDistance)
            {
                minDistance = distanceToEnemy;
                nearest = enemy;
            }
        }
        return nearest;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
                // Debug.Log($"{enemy.name} entered range. Enemies: {enemiesInRange.Count}");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy); // Hapus saat keluar trigger itu benar
                // Debug.Log($"{enemy.name} exited range. Enemies: {enemiesInRange.Count}");
            }
        }
    }
}