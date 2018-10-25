using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public float damping = 4f; 
	private BoxCollider2D cameraBox; 
	private Transform player; 
	private BoxCollider2D currentBoundary; 

	// Use this for initialization
	void Start () {
		cameraBox = GetComponent<BoxCollider2D>();
		player = GameManager.Player.transform; 
		currentBoundary = GameObject.Find("Boundary").GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		AspectRatioBoxChange(); 
		FollowPlayer(); 
	}
	
	void AspectRatioBoxChange() {
		// Debug.Log(Camera.main.aspect); 
		
		// 16:10 
		if (Camera.main.aspect >= (1.575f) && Camera.main.aspect < 1.7f) {
			cameraBox.size = new Vector2 (25.5f, 16f);
		}
		// 16:9 
		else if (Camera.main.aspect >= (1.7f) && Camera.main.aspect < 1.8f) {
			cameraBox.size = new Vector2 (28.5f, 16f);
		} 
		// 5:4
		else if (Camera.main.aspect >= (1.25f) && Camera.main.aspect < 1.3f) {
			cameraBox.size = new Vector2 (20f, 16f);
		}
		// 4:3 
		else if (Camera.main.aspect >= (1.3f) && Camera.main.aspect < 1.4f) {
			cameraBox.size = new Vector2 (21.25f, 16f);
		}
		// 3:2
		else if (Camera.main.aspect >= (1.5f) && Camera.main.aspect < 1.6f) {
			cameraBox.size = new Vector2 (24f, 16f);
		}
		
		// change else if to if?? 
	}
	
	void FollowPlayer() {
		//  exit function if no active boundary  
		if (!GameObject.Find("Boundary")) return; 
		
		BoxCollider2D newBoundary = GameObject.Find("Boundary").GetComponent<BoxCollider2D>();
		Vector3 newPosition = new Vector3 (	Mathf.Clamp(player.position.x, newBoundary.bounds.min.x + cameraBox.size.x / 2, newBoundary.bounds.max.x - cameraBox.size.x / 2),
									Mathf.Clamp(player.position.y, newBoundary.bounds.min.y + cameraBox.size.y / 2, newBoundary.bounds.max.y - cameraBox.size.y / 2),
									transform.position.z); 

		// when player steps into new boundary 
		if (newBoundary != currentBoundary) {
			transform.position = newPosition; 
		}
		else transform.position = Vector3.Lerp(transform.position, newPosition, damping * Time.deltaTime); 
		currentBoundary = newBoundary; 
	}
}
