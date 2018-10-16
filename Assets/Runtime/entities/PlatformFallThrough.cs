using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider2D))]
public class PlatformFallThrough : MonoBehaviour {

	private BoxCollider2D boxCollider2DComponent;

	// Use this for initialization
	void Start () {
		boxCollider2DComponent = GetComponent<BoxCollider2D>();

		Assert.AreEqual(LayerMask.NameToLayer("Ground"), this.gameObject.layer,
						"PlatformFallThrough should be on Ground layer");
	}

	// Update is called once per frame
	void Update () {

	}

	public void AllowFallThrough(GameObject requester) {
		SetFallThrough(requester, true);
	}

	public void EndFallThrough(GameObject requester) {
		SetFallThrough(requester, false);
	}

	private void SetFallThrough(GameObject requester, bool isFallThrough) {
		var requesterCollider = requester.GetComponent<BoxCollider2D>();
		if(!requesterCollider) {
			throw new ArgumentException("requester does not have BoxCollider2D");
		}

		Physics2D.IgnoreCollision(requesterCollider, boxCollider2DComponent, isFallThrough);
	}
}
