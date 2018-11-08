using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GroundEnemy : MonoBehaviour {
	
	public Animator anim;
    private IEnemyState currentState; 
    public GameObject Target { get; set; }

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
	protected float speed;
	[SerializeField]
	protected float distance;
	
	protected  bool facingRight;
	
	// Use this for initialization
	public virtual void Start () {		
		facingRight = true;
        meleeRange = 2; 
		anim = GetComponent<Animator>();

        ChangeState(new IdleState()); 
	}

	// Update is called once per frame
	void Update () {
        currentState.Execute();

        LookAtTarget(); 
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
        transform.Translate(GetDirection() * speed * Time.deltaTime);  


    }

    public Vector2 GetDirection()
    {
        Vector2 dir = facingRight ? Vector2.right : Vector2.left;
        return dir; 
    }

    private void MeleeAtack()
    {

    }
}
