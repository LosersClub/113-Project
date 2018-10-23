using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sheep : MonoBehaviour {
	
	public float speed = 3f;
	public float distance = 1f;
	
	private bool facingRight = true; 
	private Vector2 dir = Vector2.right; 
	
	public Transform groundPoint; 
	public Transform wallPoint; 
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Vector2.right * speed * Time.deltaTime); 
		
		Debug.DrawRay(groundPoint.position, Vector2.down*distance); 
		Debug.DrawRay(wallPoint.position, dir*distance); 
		
		RaycastHit2D groundHit = Physics2D.Raycast(groundPoint.position, Vector2.down, distance); 
		RaycastHit2D wallHit = Physics2D.Raycast(wallPoint.position, dir, distance); 
		
		if(groundHit == false) {
			Flip();
			dir *= -1; 
		}
		
		else if(wallHit == true) {
			Flip();
			dir *= -1; 
		}
	}
	
	void Flip() {
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
