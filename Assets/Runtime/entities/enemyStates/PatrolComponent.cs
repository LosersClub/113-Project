using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolComponent : MonoBehaviour {
    private EnemyComponent enemy;
    private float flipTimer; 
    private float flipCooldown = 1f;


    public Transform groundPoint;
    public Transform wallPoint;

    public float speed = 3f;
    public float distance = 0.5f;



    // Use this for initialization
    void Start () {
        enemy = GetComponent<EnemyComponent>();
	}
	
	// Update is called once per frame
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
        else Move(dir); 	
	}

    void Move(Vector2 direction)
    {
        Animator anim = enemy.anim;
        //anim.SetFloat("speed", 1);

        transform.Translate(direction * speed * Time.deltaTime);
    
    }
}
