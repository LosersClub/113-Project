using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GroundEnemy : MonoBehaviour {
	[HideInInspector]
	public Animator anim { get; set; }

    private MeleeDamageDealer meleeAttack { get; set; }
    public GameObject Target { get; set; }

    public Transform groundPoint;
    public Transform wallPoint;
    private IEnemyState currentState;

    public float meleeRange;
    public bool InMeleeRange
    {
        get
        {
            if (Target != null)
                return Vector2.Distance(transform.position, Target.transform.position) <= meleeRange;
            else
                return false; 
        }
    }

	[SerializeField]
	public float speed;
	[SerializeField]
	public float distance;
    [SerializeField]
    protected float front_sight;
    [SerializeField]
    protected float behind_sight;
    [SerializeField]
    protected float vertical_sight;

    private bool facingRight; 
	
	// Use this for initialization
	public virtual void Start () {
        anim = GetComponent<Animator>();
        meleeAttack = GetComponent<MeleeDamageDealer>();

        speed = 3;
        distance = 0.5f; 
        //front_sight = 8;
        //behind_sight = 2;
        //vertical_sight = 2; 

        meleeRange = 2;
        facingRight = true;

        ChangeState(new IdleState()); 
	}

	// Update is called once per frame
	void Update () {
        currentState.Execute();

        DetectTarget(); 
        LookAtTarget(); 
	}

    private void DetectTarget()
    {
        float xDelta = GameManager.Player.transform.position.x - transform.position.x;
        float yDelta = GameManager.Player.transform.position.y - transform.position.y;

        Debug.DrawRay(transform.position, GetDirection() * front_sight);
        Debug.DrawRay(transform.position, GetDirection() * behind_sight * -1); 

        if (yDelta <= vertical_sight)
        {
            if (xDelta > -behind_sight && xDelta < front_sight && facingRight ||
                xDelta < behind_sight && xDelta > -front_sight && !facingRight)
            {
                Target = GameManager.Player.gameObject;
            }
            else
                Target = null;
        }
    }

    private void LookAtTarget()
    {
        if (Target != null)
        {
            float xDelta = Target.transform.position.x - transform.position.x;
            if (xDelta < 0 && facingRight || xDelta > 0 && !facingRight) Flip();
        }
    }

    public void ChangeState(IEnemyState newState)
    {
        if (currentState != null) currentState.Exit();
        currentState = newState;
        currentState.Enter(this); 
    }

	public void Flip() {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1); 
	}

    public virtual void Move()
    {
        anim.SetFloat("speed", 1);
        Vector2 dir = GetDirection();
        transform.Translate(dir * speed * Time.deltaTime);
        
        Debug.DrawRay(groundPoint.position, Vector2.down * distance);
        Debug.DrawRay(wallPoint.position, dir * distance);

        RaycastHit2D groundHit = Physics2D.Raycast(groundPoint.position, Vector2.down, distance);
        RaycastHit2D wallHit = Physics2D.Raycast(wallPoint.position, dir, distance);

        if (groundHit == false)
        {
            //Debug.Log("no ground"); 
            Flip();
        }
        else if (wallHit == true && !wallHit.collider.CompareTag("Player"))
        {
            //Debug.Log("wall hit"); 
            Flip();
        }
    }



    public Vector2 GetDirection()
    {
        Vector2 dir = facingRight ? Vector2.right : Vector2.left;
        return dir; 
    }

    public void Attack()
    {
        Debug.Log("horizontal attacc"); 
        anim.SetTrigger("attack");
        StartCoroutine(WaitForAnimation("Attack"));         
    }

    public void Projectile()
    {

    }

    IEnumerator WaitForAnimation(string name)
    {
        if (anim)
            do
            {
                yield return null;
            } while (!anim.GetCurrentAnimatorStateInfo(0).IsName(name));

        meleeAttack.HorizontalHit(facingRight);
    }
}
