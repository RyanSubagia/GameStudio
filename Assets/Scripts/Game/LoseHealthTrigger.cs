using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseHealthTrigger : MonoBehaviour
{
    public HealthSystem healthSystem;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            healthSystem.LoseHealth();
            Destroy(other.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
