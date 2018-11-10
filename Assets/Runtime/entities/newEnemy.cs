using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newEnemy : GroundEnemy {

    public Transform groundPoint;
    public Transform wallPoint;

    // Use this for initialization
    public override void Start () {
        base.Start();
        distance = .5f; 
	}

    public override void Move()
    {
        base.Move();

        Vector2 dir = GetDirection();
        Debug.DrawRay(groundPoint.position, Vector2.down * distance);
        Debug.DrawRay(wallPoint.position, dir * distance);

        RaycastHit2D groundHit = Physics2D.Raycast(groundPoint.position, Vector2.down, distance);
        RaycastHit2D wallHit = Physics2D.Raycast(wallPoint.position, dir, distance);

        if (groundHit == false)
        {
            Debug.Log("no ground"); 
            Flip();
        }
        else if (wallHit == true)
        {
            Debug.Log("wall hit"); 
            Flip();
        } 
    }
}
