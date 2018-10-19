using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider2D))]
public class Platform : MonoBehaviour {

	private BoxCollider2D boxCollider2DPhysical = null;
	private BoxCollider2D boxCollider2DTrigger = null;

	private HashSet<Collider2D> ignoredColliders = new HashSet<Collider2D>();

	// Use this for initialization
	void Start () {
		BoxCollider2D[] boxCollider2Ds = GetComponents<BoxCollider2D>();
		Assert.AreEqual(2, boxCollider2Ds.Length);
		foreach(BoxCollider2D boxCollider2D in boxCollider2Ds) {
			if(boxCollider2D.isTrigger) {
				this.boxCollider2DTrigger = boxCollider2D;
			}
			else {
				this.boxCollider2DPhysical = boxCollider2D;
			}
		}
		Assert.IsNotNull(this.boxCollider2DPhysical);
		Assert.IsNotNull(this.boxCollider2DTrigger);

		Assert.AreEqual(LayerMask.NameToLayer("Ground"), this.gameObject.layer,
						"Platform should be on Ground layer");
	}

	// Update is called once per frame
	void Update () {

	}

	public void AllowFallThrough(GameObject requester) {
		var requesterCollider = requester.GetComponent<BoxCollider2D>();
		if(!requesterCollider) {
			throw new ArgumentException("requester does not have BoxCollider2D");
		}

		// Note: correctly handles case where requester calls AllowFallThrough
		// multiple times in succession.
		Physics2D.IgnoreCollision(requesterCollider, boxCollider2DPhysical);
		this.ignoredColliders.Add(requesterCollider);
	}

	void OnTriggerExit2D(Collider2D other) {
		if(this.ignoredColliders.Contains(other)) {
			Physics2D.IgnoreCollision(other, boxCollider2DPhysical, false);
			this.ignoredColliders.Remove(other);
		}
	}
}
