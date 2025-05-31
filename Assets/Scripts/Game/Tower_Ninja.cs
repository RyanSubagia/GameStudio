using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_Ninja : Tower
{
    //FIELDS
    //damage
    public int damage;
    //prefab (shooting item)
    public GameObject prefab_shootItem;
    //shoot interval
    public float interval;

    private List<Enemy> enemiesInRange = new List<Enemy>();
    private bool isShooting = false;


    //METHODS
    //init (start the shooting interval)
    protected override void Start()
    {
        Debug.Log("NINJA");
        //start the shooting interval IEnum
        StartCoroutine(ShootDelay());
    }

    void Update()
    {
        if (enemiesInRange.Count > 0 && !isShooting)
        {
            StartCoroutine(ShootDelay());
        }
    }

    //Interval for shooting
    IEnumerator ShootDelay()
    {
        isShooting = true;
        while (enemiesInRange.Count > 0)
        {
            ShootItem();
            yield return new WaitForSeconds(interval);
        }

        isShooting = false;
    }
    //Shoot an item
    void ShootItem()
    {
        if (enemiesInRange.Count == 0) return;

        // You can optionally target the first enemy in the list
        Enemy target = enemiesInRange[0];
        if (target != null)
        {
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;
            // Untuk 2D, rotasi dihitung agar sumbu 'up' (atau 'right' tergantung orientasi sprite) mengarah ke target
            // Jika sprite proyektil Anda defaultnya mengarah ke kanan (sumbu X positif lokal):
            float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
            Quaternion projectileRotation = Quaternion.Euler(0f, 0f, angle);

            GameObject shotItem = Instantiate(prefab_shootItem, transform.position, projectileRotation);
            shotItem.GetComponent<ShootItem>().Init(damage, target); // Target mungkin tidak lagi diperlukan di Init jika hanya untuk arah awal
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !enemiesInRange.Contains(enemy)) // Pastikan tidak duplikat
            {
                enemiesInRange.Add(enemy);
                // Anda mungkin ingin memulai menembak jika belum menembak dan ada musuh
                // if (!isShooting && enemiesInRange.Count > 0)
                // {
                // StartCoroutine(ShootDelay());
                // }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Remove(enemy);
        }
    }
}
