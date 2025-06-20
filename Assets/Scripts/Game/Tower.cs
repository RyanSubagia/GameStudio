using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public int health;
    public int cost;
    private Vector3Int cellPosition;

    public AudioClip destructionSound;
    public float destructionSoundVolume = 3f;

    public AudioClip sellSound;
    public float sellSoundVolume = 3f;

    protected virtual void Start()
    {
        Debug.Log("BASE TOWER");
    }

    public virtual void Init(Vector3Int cellPos)
    {
        cellPosition = cellPos;
    }

    //Lose Health
    public virtual bool LoseHealth(int amount)
    {
        Debug.Log(gameObject.name + " sedang menerima damage sebesar: " + amount + ". Health saat ini sebelum damage: " + health, this.gameObject);
        //health = health - amount
        health -= amount;

        if (health <= 0)
        {
            Die();
            return true;
        }
        return false;
    }
    //Die
    protected virtual void Die()
    {
        Debug.Log("Tower is dead");

        if (destructionSound != null)
        {
            AudioSource.PlayClipAtPoint(destructionSound, transform.position, destructionSoundVolume);
        }

        FindObjectOfType<Spawner>().RevertCellState(cellPosition);
        Destroy(gameObject);
    }
    //Upgrade
    public virtual void Upgrade()
    {
        Debug.Log("Tower upgraded!");
        health += 10;
    }
    //Sell
    public void Sell()
    {
        if (sellSound != null)
        {
            AudioSource.PlayClipAtPoint(sellSound, transform.position, sellSoundVolume);
        }
        Debug.Log("Tower sold!");
        GameManager.instance.currency.Gain(cost / 2);
        FindObjectOfType<Spawner>().RevertCellState(cellPosition);
        // Remove this tower
        Destroy(gameObject);
    }
    void OnMouseDown()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        FindObjectOfType<TowerInteractionUI>().ShowPanel(this, screenPos);
    }
}
