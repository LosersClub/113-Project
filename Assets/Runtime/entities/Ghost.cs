using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Ghost : MonoBehaviour {

  private BoxCollider2D boxCollider2D;
  private Rigidbody2D rigidBody2D;

  void Start () {
    this.boxCollider2D = this.GetComponent<BoxCollider2D>();
    this.rigidBody2D = this.GetComponent<Rigidbody2D>();
  }
  
  void Update () {

  }
}
