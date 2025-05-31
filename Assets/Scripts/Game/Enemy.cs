using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int baseHealth = 1;
    public int baseAttackPower = 2;
    public float baseMoveSpeed = 0.01f;
    public int baseCurrencyDropAmount = 5;

    //Health,AttackPower,MoveSpeed
    public int health,attackPower;
    public float moveSpeed;
    public int currencyDropAmount;

    public Animator animator;
    public float attackInterval;
    Coroutine attackOrder;
    Tower detectedTower;
    public HealthSystem healthsystem;
    // Panggil ini dari EnemySpawner setelah Instantiate
    public void InitializeStatsForLevel(int levelIndex) // levelIndex dimulai dari 0
    {
        //// Contoh scaling sederhana, sesuaikan formula ini!
        //currentHealth = baseHealth + (levelIndex * 2); // Misal, health +10 per level tambahan
        //currentAttackPower = baseAttackPower + (levelIndex * 1); // Attack +1 per level tambahan
        //currentMoveSpeed = baseMoveSpeed + (levelIndex * 0.01f); // Speed +0.1 per level tambahan
        //currentCurrencyDropAmount = baseCurrencyDropAmount + (levelIndex * 2); // Currency +2 per level tambahan

        //// Langsung set health saat ini
        //// health = currentHealth; // Anda sudah punya variabel 'health', jadi gunakan itu saja
        //this.health = currentHealth; // Ganti 'health' menjadi 'this.health' jika ada variabel global health lain, atau pastikan konsisten
        //                             // Jika 'health' yang di atas adalah 'currentHealth', maka tidak perlu duplikat
        //                             // Mari kita asumsikan 'health' adalah variabel yang sudah ada untuk menampung health aktual musuh.
        //                             // Maka, kita akan pakai 'health' yang sudah ada:
        //                             // public int health,attackPower; // Variabel lama Anda
        //                             // public float moveSpeed;
        //                             // public int currencyDropAmount; // Variabel lama Anda

        //// Menggunakan variabel yang sudah Anda punya:
        //this.health = baseHealth + (levelIndex * 2);
        //Debug.Log(gameObject.name + " - Level: " + (levelIndex + 1) +
        //  " - Initialized health: " + this.health +
        //  " (baseHealth: " + this.baseHealth + ")");
        //this.attackPower = baseAttackPower + (levelIndex * 1);
        //this.moveSpeed = baseMoveSpeed + (levelIndex * 0.01f);
        //Debug.Log(gameObject.name + " - Level: " + (levelIndex + 1) +
        //  " - Initialized moveSpeed: " + this.moveSpeed +
        //  " (baseMoveSpeed: " + this.baseMoveSpeed + ")");
        //this.currencyDropAmount = baseCurrencyDropAmount + (levelIndex * 2);

        // Debug.Log(gameObject.name + " diinisialisasi untuk level " + (levelIndex + 1) + 
        //           ". HP: " + this.health + ", ATK: " + this.attackPower);
        // Langsung hitung dan tetapkan nilai ke field operasional Anda
        this.health = baseHealth + (levelIndex * 2);
        Debug.Log(gameObject.name + " - Level: " + (levelIndex + 1) +
                  " - Initialized health: " + this.health +
                  " (baseHealth: " + this.baseHealth + ")");

        this.attackPower = baseAttackPower + (levelIndex * 1);

        this.moveSpeed = baseMoveSpeed + (levelIndex * 0.01f);
        Debug.Log(gameObject.name + " - Level: " + (levelIndex + 1) +
                  " - Initialized moveSpeed: " + this.moveSpeed +
                  " (baseMoveSpeed: " + this.baseMoveSpeed + ")");

        this.currencyDropAmount = baseCurrencyDropAmount + (levelIndex * 2);

        // Debug.Log(gameObject.name + " diinisialisasi untuk level " + (levelIndex + 1) + 
        //           ". HP: " + this.health + ", ATK: " + this.attackPower);
    }


    void Update()
    {
        if(!detectedTower)
        {
            Move();
        }        
    }

    IEnumerator Attack()
    {
        animator.Play("Attack",0,0);
        //Wait attackInterval 
        yield return new WaitForSeconds(attackInterval);
        //Attack Again
        attackOrder = StartCoroutine(Attack());
    }

    //Moving forward
    void Move()
    {
        animator.Play("Move");
        transform.Translate(-transform.right*moveSpeed*Time.deltaTime);
    }

    public void InflictDamage()
    {
        bool towerDied = detectedTower.LoseHealth(attackPower);

        if (towerDied)
        {
            detectedTower = null;
            StopCoroutine(attackOrder);
        }
    }

    //Lose health
    public void LoseHealth(int amount)
    {
        health -= amount;
        //Blink Red animation
        StartCoroutine(BlinkRed());
        //Check if health is zero => destroy enemy
        if (health <= 0)
        {
            // --- BAGIAN UNTUK MENAMBAH CURRENCY ---
            if (GameManager.instance != null && GameManager.instance.currency != null)
            {
                //int dropAmount = 2;
                GameManager.instance.currency.Gain(currencyDropAmount);
                GameManager.instance.RegisterEnemyKilled();
                Debug.Log(gameObject.name + " dikalahkan dan memberikan " + currencyDropAmount + " currency."); // Untuk debugging
            }
            else
            {
                Debug.LogError("Referensi GameManager.instance atau GameManager.instance.currency tidak ditemukan saat mencoba memberikan currency dari " + gameObject.name);
            }
            // ------------------------------------

            if (attackOrder != null)
            {
                StopCoroutine(attackOrder);
            }
            Destroy(gameObject);
        }
    }

    IEnumerator BlinkRed()
    {
        //Change the spriterendere color to red
        GetComponent<SpriteRenderer>().color=Color.red;
        //Wait for really small amount of time 
        yield return new WaitForSeconds(0.2f);
        //Revert to default color
        GetComponent<SpriteRenderer>().color=Color.white;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (detectedTower)
            return;

        if (collision.tag == "Tower")
        {
            detectedTower = collision.GetComponent<Tower>();
            attackOrder = StartCoroutine(Attack());
        }

        //if (collision.CompareTag("Tower") && !detectedTower)
        //{
        //    detectedTower = collision.GetComponent<Tower>();
        //    attackOrder = StartCoroutine(Attack());
        //}
        //else if (collision.CompareTag("Base"))
        //{
        //    // This is where the enemy reaches the player's base
        //    healthsystem.LoseHealth();
        //    Destroy(gameObject);
        //}
    }
}
