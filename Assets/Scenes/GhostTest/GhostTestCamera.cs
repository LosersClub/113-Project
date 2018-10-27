using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTestCamera : MonoBehaviour {

	public float moveSpeedMultiplier = 0.5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = this.transform.position + moveSpeedMultiplier * new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
	}
}
