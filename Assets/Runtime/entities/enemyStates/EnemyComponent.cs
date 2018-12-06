using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class EnemyComponent : MonoBehaviour {
    private MovementController controller;

    [SerializeField]
    private Animator anim;

    // Convenience property for other components:
    public Animator Anim {
        get {
            return this.anim;
        }
    }

    public float PlayerDeltaX
    {
        get
        {
            return GameManager.Player.transform.position.x - transform.position.x; 
        }
    }
    public float PlayerDeltaY
    {
        get
        {
            return GameManager.Player.transform.position.y - transform.position.y;
        }
    }
    public float PlayerDistance {
        get {
            return (GameManager.Player.transform.position - transform.position).magnitude;
        }
    }

    // flags 
    private bool facingRight;
    public bool FacingRight {
        get {
            return this.facingRight;
        }
    }
    // mutex allowing one action at a time 
    [System.NonSerialized]
    public bool inAction; 

    // movement transforms 
    [SerializeField]
    private Transform groundPoint;
    [SerializeField]
    private Transform wallPoint;
    [SerializeField]
    private float margin = 1f;

    void Start () {
        controller = GetComponent<MovementController>();
        facingRight = true;
        inAction = false; 
	}

    void Update()
    {
        Vector2 newVelocity = this.controller.Velocity + Physics2D.gravity * Time.deltaTime;
        this.controller.Move(newVelocity * Time.deltaTime);
    }

    public Vector2 GetDirection()
    {
        Vector2 dir = facingRight ? Vector2.right : Vector2.left;
        return dir;
    }

    public void LookAtTarget()
    {
        if (PlayerDeltaX < 0 && facingRight || PlayerDeltaX > 0 && !facingRight) Flip();
    }

    public void Flip()
    {
        facingRight = !facingRight;

        // Changes scale so that groundPoint and wallPoint also face correct direction:
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    /// <summary>
    /// Sets speed to argument EXCEPT when in midair or would collide with wall.
    /// </summary>
    public void SetSpeed(float speed)
    {
        int impassableMask = LayerMask.GetMask("Impassable");
        int platformMask = LayerMask.GetMask("Platform");
        int layerMask = impassableMask | platformMask;

        Vector2 dir = GetDirection();
        Debug.DrawRay(groundPoint.position, Vector2.down * margin);
        //Debug.DrawRay(wallPoint.position, dir * margin);
        RaycastHit2D groundHit = Physics2D.Raycast(groundPoint.position, Vector2.down, margin, layerMask);
        //RaycastHit2D wallHit = Physics2D.Raycast(wallPoint.position, dir, margin, layerMask);

        if (groundHit == false)
        {
            Debug.Log("no ground"); 
            setVelocityX(0);
        }
        /*
        else if (wallHit == true && !wallHit.collider.CompareTag("Player"))
        {
            setVelocityX(0);
        }
        */
        else
        {
            setVelocityX(speed * (facingRight ? 1: -1));
        }
    }

    private void setVelocityX(float speed)
    {
        this.controller.Velocity.x = speed; 
    }
}
