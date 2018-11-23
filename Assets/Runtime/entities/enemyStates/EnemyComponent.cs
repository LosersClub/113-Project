using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyComponent : MonoBehaviour {
    public Animator anim;
    public float deltaX
    {
        get
        {
            return GameManager.Player.transform.position.x - transform.position.x; 
        }
    }
    public float deltaY
    {
        get
        {
            return GameManager.Player.transform.position.y - transform.position.y;
        }
    }

    // flags 
    public bool facingRight;
    // mutex allowing one action at a time 
    public bool inAction; 

    // movement transforms 
    public Transform groundPoint;
    public Transform wallPoint;
    public float margin = 1f; 

    void Start () {
        facingRight = true;
        inAction = false; 
	}

    public Vector2 GetDirection()
    {
        Vector2 dir = facingRight ? Vector2.right : Vector2.left;
        return dir;
    }

    public void LookAtTarget()
    {
        if (deltaX < 0 && facingRight || deltaX > 0 && !facingRight) Flip();
    }

    public void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
    }

    public void Move(float speed)
    {
        Vector2 dir = GetDirection();
        Debug.DrawRay(groundPoint.position, Vector2.down * margin);
        Debug.DrawRay(wallPoint.position, dir * margin);
        RaycastHit2D groundHit = Physics2D.Raycast(groundPoint.position, Vector2.down, margin);
        RaycastHit2D wallHit = Physics2D.Raycast(wallPoint.position, dir, margin);

        if (groundHit == false)
        {
            //Debug.Log("no ground");
        }
        else if (wallHit == true && !wallHit.collider.CompareTag("Player"))
        {
            //Debug.Log("wall");
        }
        else
            transform.Translate(GetDirection() * speed * Time.deltaTime);
    }

}
