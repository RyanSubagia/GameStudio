using UnityEngine;
using System.Collections.Generic; // Untuk List jika Anda ingin mencegah multi-hit kompleks

public class Arrow : MonoBehaviour
{
    public float speed = 15f; // Kecepatan panah, bisa diatur di Inspector prefab panah
    private Enemy initialTarget; // Target awal untuk mengarahkan panah
    private int damage;
    private int hitCount = 0;
    private const int MAX_ENEMIES_TO_HIT = 3; 
    public void Initialize(int dmg, Enemy target)
    {
        this.damage = dmg;
        this.initialTarget = target;

        if (initialTarget != null)
        {
            Vector3 direction = (initialTarget.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle); 
        }
    }

    void Update()
    {
        transform.Translate(transform.right * speed * Time.deltaTime, Space.World);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {


            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log($"Arrow hit {enemy.name}, damage: {damage}, hitCount before this: {hitCount}");
                enemy.LoseHealth(damage);

                hitCount++;

                if (hitCount >= MAX_ENEMIES_TO_HIT)
                {
                    Destroy(gameObject);
                }
            }
        }

        else if (other.CompareTag("Out") || other.CompareTag("Obstacle")) 
        {
            Destroy(gameObject);
        }
    }
}