using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Collider2D))]
public class Platform : MonoBehaviour {

  private Collider2D collider2DPhysical = null;
  private Collider2D collider2DTrigger = null;

  private HashSet<Collider2D> ignoredColliders = new HashSet<Collider2D>();

  // Use this for initialization
  void Start () {
    Collider2D[] collider2Ds = GetComponents<Collider2D>();
    Assert.AreEqual(2, collider2Ds.Length);
    foreach(Collider2D collider2D in collider2Ds) {
      if(collider2D.isTrigger) {
        this.collider2DTrigger = collider2D;
      }
      else {
        this.collider2DPhysical = collider2D;
      }
    }
    Assert.IsNotNull(this.collider2DPhysical);
    Assert.IsNotNull(this.collider2DTrigger);

    Assert.AreEqual(LayerMask.NameToLayer("Ground"), this.gameObject.layer,
            "Platform should be on Ground layer");
  }

  // Update is called once per frame
  void Update () {

  }

  public void AllowFallThrough(GameObject requester) {
    var requesterCollider = requester.GetComponent<Collider2D>();
    if(!requesterCollider) {
      throw new ArgumentException("requester does not have Collider2D");
    }

    // Note: correctly handles case where requester calls AllowFallThrough
    // multiple times in succession.
    Physics2D.IgnoreCollision(requesterCollider, collider2DPhysical);
    this.ignoredColliders.Add(requesterCollider);
  }

  void OnTriggerExit2D(Collider2D other) {
    if(this.ignoredColliders.Contains(other)) {
      Physics2D.IgnoreCollision(other, collider2DPhysical, false);
      this.ignoredColliders.Remove(other);
    }
  }
}
