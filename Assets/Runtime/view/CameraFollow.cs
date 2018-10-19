using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	private BoxCollider2D cameraBox; 
	private Transform player; 

	// Use this for initialization
	void Start () {
		cameraBox = GetComponent<BoxCollider2D>();
    player = GameManager.Player.transform; 
	}
	
	// Update is called once per frame
	void Update () {
		AspectRatioBoxChange(); 
		FollowPlayer(); 
	}
	
	void AspectRatioBoxChange() {
		Debug.Log(Camera.main.aspect); 
		
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
		if (GameObject.Find("Boundary")) {
			BoxCollider2D boxCollider = GameObject.Find("Boundary").GetComponent<BoxCollider2D>();
			transform.position = new Vector3 (	Mathf.Clamp(player.position.x, boxCollider.bounds.min.x + cameraBox.size.x / 2, boxCollider.bounds.max.x - cameraBox.size.x / 2),
												Mathf.Clamp(player.position.y, boxCollider.bounds.min.y + cameraBox.size.y / 2, boxCollider.bounds.max.y - cameraBox.size.y / 2),
												transform.position.z); 
		}
	}
}
