using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tower_Ballista : Tower // Pastikan Tower.cs adalah kelas dasar Anda
{
    public int arrowDamage; // Damage lebih rendah dari cannon
    public GameObject prefab_Arrow; // Prefab untuk panah
    public float interval; // Interval menembak ballista
    public Transform shootPoint; // Titik keluar panah

    private List<Enemy> enemiesInRange = new List<Enemy>();
    private bool isShooting = false;
    private Animator ballistaAnimator; // Animator untuk animasi ballista
    private Enemy currentTargetForArrowAnimation; // Target untuk Animation Event

    protected override void Start()
    {
        base.Start(); // Panggil Start dari kelas dasar jika ada
        ballistaAnimator = GetComponent<Animator>();
        if (ballistaAnimator == null)
        {
            Debug.LogError("Animator component not found on Tower_Ballista!", this.gameObject);
        }
    }

    void Update()
    {
        if (!isShooting && enemiesInRange.Count > 0)
        {
            RemoveNullEnemies();
            if (enemiesInRange.Count > 0)
            {
                StartCoroutine(ShootDelay());
            }
        }
    }

    IEnumerator ShootDelay()
    {
        isShooting = true;
        while (enemiesInRange.Count > 0)
        {
            PrepareToShootArrow();
            yield return new WaitForSeconds(interval);
            RemoveNullEnemies();
            if (enemiesInRange.Count == 0)
            {
                break;
            }
        }
        isShooting = false;
    }

    void PrepareToShootArrow()
    {
        if (enemiesInRange.Count == 0)
        {
            currentTargetForArrowAnimation = null;
            return;
        }

        Enemy target = GetNearestEnemy();

        if (target != null && target.gameObject.activeInHierarchy)
        {
            currentTargetForArrowAnimation = target;
            if (ballistaAnimator != null)
            {
                ballistaAnimator.SetTrigger("Shoot"); // Gunakan trigger "Shoot" yang sama atau buat baru
            }
            else
            {
                // Fallback jika animator tidak ada, langsung tembak (meski tidak ideal)
                // FireArrowFromAnimationEvent(); // Hati-hati jika dipanggil langsung tanpa animasi
            }
        }
        else
        {
            currentTargetForArrowAnimation = null;
        }
    }

    // METODE INI AKAN DIPANGGIL OLEH ANIMATION EVENT DARI ANIMASI BALLISTA
    public void FireArrowFromAnimationEvent()
    {
        if (currentTargetForArrowAnimation == null || !currentTargetForArrowAnimation.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Ballista: Target for animation event is null or inactive. Arrow not fired.");
            return;
        }

        if (prefab_Arrow == null)
        {
            Debug.LogError("Ballista: prefab_Arrow is NOT ASSIGNED in the Inspector!");
            return;
        }

        Vector3 projectileSpawnPosition = (shootPoint != null) ? shootPoint.position : transform.position;
        Vector3 directionToTarget = (currentTargetForArrowAnimation.transform.position - projectileSpawnPosition).normalized;
        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        Quaternion projectileRotation = Quaternion.Euler(0f, 0f, angle);

        GameObject shotArrowObject = Instantiate(prefab_Arrow, projectileSpawnPosition, projectileRotation);
        Arrow arrowScript = shotArrowObject.GetComponent<Arrow>();

        if (arrowScript != null)
        {
            arrowScript.Initialize(arrowDamage, currentTargetForArrowAnimation);
        }
        else
        {
            Debug.LogError($"Arrow script NOT FOUND on instantiated arrow prefab '{shotArrowObject.name}'!");
        }
    }

    // Metode GetNearestEnemy dan RemoveNullEnemies bisa sama persis dengan di Tower_Cannon.cs
    void RemoveNullEnemies()
    {
        enemiesInRange.RemoveAll(enemy => enemy == null || !enemy.gameObject.activeInHierarchy);
    }

    Enemy GetNearestEnemy()
    {
        Enemy nearest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Enemy enemy in enemiesInRange)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;
            float distanceToEnemy = Vector3.Distance(enemy.transform.position, currentPosition);
            if (distanceToEnemy < minDistance)
            {
                minDistance = distanceToEnemy;
                nearest = enemy;
            }
        }
        return nearest;
    }

    // Metode OnTriggerEnter2D dan OnTriggerExit2D juga bisa sama
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // Pastikan tag musuh Anda "Enemy"
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
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
                enemiesInRange.Remove(enemy);
            }
        }
    }
}