using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GroundEnemy : MonoBehaviour {
	
	protected Animator anim; 
	
	[SerializeField]
	protected float speed;
	[SerializeField]
	protected float distance;
	
	protected  bool facingRight;
	
	// Use this for initialization
	// void Start () {		
	// }
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void Flip() {
		if(facingRight == true) {
				transform.eulerAngles = new Vector3(0, -180, 0); 
				facingRight = false; 
		}
		else {
			transform.eulerAngles = new Vector3(0, 0, 0); 
			facingRight = true; 
		}
	}
}
