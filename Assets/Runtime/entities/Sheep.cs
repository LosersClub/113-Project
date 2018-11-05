using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : GroundEnemy {
	
	public Transform groundPoint; 
	public Transform wallPoint; 
	
	// Use this for initialization
	public override void Start () {
		base.Start(); 
		speed = 3f;
		distance = .5f;
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Vector2.right * speed * Time.deltaTime); 
		
		Vector2 dir = facingRight ? Vector2.right : Vector2.left; 
		Debug.DrawRay(groundPoint.position, Vector2.down*distance); 
		Debug.DrawRay(wallPoint.position, dir*distance); 
		
		RaycastHit2D groundHit = Physics2D.Raycast(groundPoint.position, Vector2.down, distance); 
		RaycastHit2D wallHit = Physics2D.Raycast(wallPoint.position, dir, distance); 
		
		if(groundHit == false) {
			Flip();
		}
		
		else if(wallHit == true) {
			Flip();
		}
	}
}
