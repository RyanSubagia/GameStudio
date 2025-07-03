using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class Tower_Ballista : Tower
{
    public int arrowDamage; 
    public GameObject prefab_Arrow; 
    public float interval; 
    public Transform shootPoint; 

    public AudioClip fireSound;
    private AudioSource audioSource; 
    public float fireSoundVolume = 0.8f;

    private List<Enemy> enemiesInRange = new List<Enemy>();
    private bool isShooting = false;
    private Animator ballistaAnimator; 
    private Enemy currentTargetForArrowAnimation; 

    protected override void Start()
    {
        base.Start(); 
        ballistaAnimator = GetComponent<Animator>();
        if (ballistaAnimator == null)
        {
            Debug.LogError("Animator component not found on Tower_Ballista!", this.gameObject);
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.playOnAwake = false; 
            audioSource.volume = fireSoundVolume;
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
                ballistaAnimator.SetTrigger("Shoot"); 
            }
            else
            {
 
            }
        }
        else
        {
            currentTargetForArrowAnimation = null;
        }
    }

    public void FireArrowFromAnimationEvent()
    {
        if (audioSource != null && fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }

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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) 
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