using UnityEngine;

public class Wall : Tower
{

    protected override void Start()
    {
        Debug.Log("Wall placed. Health: " + this.health);
    }
}