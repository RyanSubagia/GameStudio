using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public int health;
    public int cost;
    private Vector3Int cellPosition;


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
