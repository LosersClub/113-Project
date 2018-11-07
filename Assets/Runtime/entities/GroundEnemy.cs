using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GroundEnemy : MonoBehaviour {
	
	public Animator anim;
    private IEnemyState currentState; 

	[SerializeField]
	protected float speed;
	[SerializeField]
	protected float distance;
	
	protected  bool facingRight;
	
	// Use this for initialization
	public virtual void Start () {		
		facingRight = true; 
		anim = GetComponent<Animator>();

        ChangeState(new IdleState()); 
	}
	
	// Update is called once per frame
	void Update () {
        currentState.Execute(); 
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
}
