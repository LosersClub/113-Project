﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Applies constant downward movement due to gravity. Uses MovementController.
/// </summary>

[RequireComponent(typeof(MovementController))]
public class GravityMovement : MonoBehaviour {

  private MovementController movementController;

  void Start () {
    this.movementController = this.GetComponent<MovementController>();
  }
  
  void Update () {
    
  }

  void FixedUpdate() {
    Vector2 newVelocity = this.movementController.Velocity + Physics2D.gravity * Time.deltaTime;
    Debug.LogFormat("newVelocity = {0}", newVelocity);
    this.movementController.Move(newVelocity * Time.deltaTime);
  }
}
