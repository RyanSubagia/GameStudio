using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int baseHealth = 1;
    public int baseAttackPower = 2;
    public float baseMoveSpeed = 0.01f;
    public int baseCurrencyDropAmount = 5;

    public int health, attackPower;
    public float moveSpeed;
    public int currencyDropAmount;

    public float attackInterval;

    [Header("Animation Settings")]
    public bool hasDieAnimation = false; 
    public float dieAnimationDuration = 2f;
    public AudioClip dieSound;
    public float dieSoundVolume = 5f;

    public AudioClip attackSound; // Assign suara serangan di Inspector
    private AudioSource audioSource; // Referensi ke AudioSource
    public float attackSoundVolume = 5f;

    private Animator animator;
    Coroutine attackOrder;
    Tower detectedTower;

    void Awake()
    {
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
        }
    }

    public void InitializeStatsForLevel(int levelIndex)
    {
        this.health = baseHealth + (levelIndex * 2);
        this.attackPower = baseAttackPower + (levelIndex * 1);
        this.moveSpeed = baseMoveSpeed + (levelIndex * 0.01f);
        this.currencyDropAmount = baseCurrencyDropAmount + (levelIndex * 2);
    }

    void Update()
    {
        if (health > 0 && !detectedTower)
        {
            Move();
        }
    }

    IEnumerator Attack()
    {
        while (detectedTower != null && health > 0)
        {
            if (animator != null) animator.SetBool("IsAttacking", true);
            yield return new WaitForSeconds(attackInterval);
        }
    }

    void Move()
    {
        if (animator != null) animator.SetBool("IsAttacking", false);
        transform.Translate(-transform.right * moveSpeed * Time.deltaTime);
    }

    public void InflictDamage()
    {
        if (detectedTower == null || health <= 0) return;

        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound, attackSoundVolume);
        }

        bool towerDied = detectedTower.LoseHealth(attackPower);
        if (towerDied)
        {
            detectedTower = null;
            if (animator != null) animator.SetBool("IsAttacking", false);
            if (attackOrder != null)
            {
                StopCoroutine(attackOrder);
                attackOrder = null;
            }
        }
    }

    public void LoseHealth(int amount)
    {
        if (health <= 0) return;
        health -= amount;
        StartCoroutine(BlinkRed());
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        if (attackOrder != null)
        {
            StopCoroutine(attackOrder);
        }

        if (GameManager.instance != null && GameManager.instance.currency != null)
        {
            GameManager.instance.currency.Gain(currencyDropAmount);
            GameManager.instance.RegisterEnemyKilled();
        }

        if (dieSound != null)
        {
            AudioSource.PlayClipAtPoint(dieSound, transform.position, dieSoundVolume);
        }

        if (hasDieAnimation && animator != null)
        {
            animator.SetTrigger("Die");
            Destroy(gameObject, dieAnimationDuration);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator BlinkRed()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.2f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (health <= 0 || detectedTower) return;
        if (collision.CompareTag("Tower"))
        {
            detectedTower = collision.GetComponent<Tower>();
            if (animator != null) animator.SetBool("IsAttacking", true);
            attackOrder = StartCoroutine(Attack());
        }
    }
}