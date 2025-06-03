using UnityEngine;
using System.Collections.Generic; // Untuk List jika Anda ingin mencegah multi-hit kompleks

public class Arrow : MonoBehaviour
{
    public float speed = 15f; // Kecepatan panah, bisa diatur di Inspector prefab panah
    private Enemy initialTarget; // Target awal untuk mengarahkan panah
    private int damage;
    private int hitCount = 0;
    private const int MAX_ENEMIES_TO_HIT = 3; // Akan hancur setelah mengenai musuh ke-3 (menembus 2 musuh)

    // Opsional: Untuk mencegah panah mengenai musuh yang sama berkali-kali jika tembusannya cepat
    // private List<Collider2D> alreadyHitColliders = new List<Collider2D>();

    // Panggil ini dari Tower_Ballista setelah Instantiate
    public void Initialize(int dmg, Enemy target)
    {
        this.damage = dmg;
        this.initialTarget = target;

        // Langsung arahkan panah ke target awal jika ada
        if (initialTarget != null)
        {
            Vector3 direction = (initialTarget.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle); // Asumsi sprite panah menghadap ke kanan
        }
    }

    void Update()
    {
        // Gerakan lurus ke depan berdasarkan rotasi awal
        // (transform.right mengacu pada sumbu X lokal, yang seharusnya menjadi arah "depan" panah setelah dirotasi)
        transform.Translate(transform.right * speed * Time.deltaTime, Space.World);

        // Anda bisa menambahkan logika untuk menghancurkan panah jika keluar batas layar
        // Misalnya dengan trigger "OutOfBounds" seperti di skrip ShootItem Anda sebelumnya.
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Opsional: Cek jika collider musuh ini sudah pernah kena oleh panah ini
            // if (alreadyHitColliders.Contains(other))
            // {
            //     return; // Sudah kena, abaikan
            // }

            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log($"Arrow hit {enemy.name}, damage: {damage}, hitCount before this: {hitCount}");
                enemy.LoseHealth(damage); // Asumsi Enemy.cs punya metode LoseHealth(int amount)
                // alreadyHitColliders.Add(other); // Tandai sudah kena

                hitCount++;

                if (hitCount >= MAX_ENEMIES_TO_HIT)
                {
                    Destroy(gameObject); // Hancur setelah mengenai jumlah musuh maksimum
                }
                // Jika belum mencapai MAX_ENEMIES_TO_HIT, panah akan terus melaju (efek tembus)
            }
        }
        // Hancurkan panah jika mengenai objek lain yang bukan musuh (misalnya tembok atau batas)
        else if (other.CompareTag("Out") || other.CompareTag("Obstacle")) // Sesuaikan dengan tag Anda
        {
            Destroy(gameObject);
        }
    }
}