using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class TransitionManager : MonoBehaviour {

	public Animator transitionAnim; 
	public string sceneName; 
	
	private BoxCollider2D currentBoundary; 
	
	void Start () {
		currentBoundary = GameObject.Find("Boundary").GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
				transitionAnim.SetTrigger("fade_out"); 
				
		if (!GameObject.Find("Boundary")) return; 
		
		BoxCollider2D newBoundary = GameObject.Find("Boundary").GetComponent<BoxCollider2D>();
		// when player steps into new boundary 

		if (newBoundary != currentBoundary) {
			StartCoroutine(LoadCell()); 
		}
		currentBoundary = newBoundary; 
	}
	
	IEnumerator LoadCell() {
		transitionAnim.SetTrigger("exit"); 
		yield return new WaitForSeconds(1.5f);
		SceneManager.LoadScene(sceneName); 
		
	}
	
	
}
