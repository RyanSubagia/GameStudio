using UnityEngine;

public class ShootItem : MonoBehaviour
{

    public Transform graphics;
    public int damage;
    public float flySpeed,rotateSpeed;

    Enemy target;


    public void Init(int dmg, Enemy target)
    {
        this.damage = dmg;
        this.target = target;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Enemy")
        {
            Debug.Log("Shot the enemy");
            collision.GetComponent<Enemy>().LoseHealth(this.damage);
            Destroy(gameObject);
        }
        if (collision.tag == "Out")
        {            
            Destroy(gameObject);
        }
    }
    void Update()
    {
        Rotate();
        FlyForward();
    }
    void Rotate()
    {
        graphics.Rotate(new Vector3(0,0,-rotateSpeed*Time.deltaTime));
    }
    void FlyForward()
    {
        if (target != null)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            transform.position += direction * flySpeed * Time.deltaTime;


            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            graphics.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
 
            transform.Translate(transform.right * flySpeed * Time.deltaTime); 
        }
    }

}
