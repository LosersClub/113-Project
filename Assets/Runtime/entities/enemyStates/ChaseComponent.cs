using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseComponent : MonoBehaviour {
    private EnemyComponent enemy;
    private float flipTimer;
    private float flipCooldown
    {
        get
        {
            return UnityEngine.Random.Range(.25f, 1);
        }
    }

    public Transform groundPoint;
    public Transform wallPoint;

    public float speed = 3f;
    public float distance = 0.5f;

    void Start () {
        enemy = GetComponent<EnemyComponent>();
	}
	
	void Update () {
        flipTimer += Time.deltaTime;
        if (flipTimer >= flipCooldown)
        {
            enemy.LookAtTarget();
            flipTimer = 0; 
        }
        Vector2 dir = enemy.GetDirection();

        Debug.DrawRay(groundPoint.position, Vector2.down * distance);
        Debug.DrawRay(wallPoint.position, dir * distance);
        RaycastHit2D groundHit = Physics2D.Raycast(groundPoint.position, Vector2.down, distance);
        RaycastHit2D wallHit = Physics2D.Raycast(wallPoint.position, dir, distance);

        if (groundHit == false)
        {
            //enemy.Flip();
        }
        else if (wallHit == true && !wallHit.collider.CompareTag("Player"))
        {
            //enemy.Flip();
        }
        else if (enemy.canMove)
            Move(dir); 	
	}

    void Move(Vector2 direction)
    {
        Animator anim = enemy.anim;
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
