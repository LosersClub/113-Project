using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Ghost : MonoBehaviour {

  public float driftSpeedMultiplier = 2.0f;
  public float barrierDetectionDistance = 0.5f;
  public float barrierMinDistance = 0.1f;
  public float barrierSlowdownMultiplier = 0.1f;

  private const float BarrierDetectionWidth = 0.05f;

  private BoxCollider2D boxCollider2D;
  private Rigidbody2D rigidBody2D;
  private SpriteRenderer spriteRenderer;

  private Vector2 velocity;
  private bool facingRight = true;

  void Start () {
    Assert.IsTrue(barrierMinDistance < barrierDetectionDistance);

    this.boxCollider2D = this.GetComponent<BoxCollider2D>();
    this.rigidBody2D = this.GetComponent<Rigidbody2D>();
    this.spriteRenderer = this.GetComponent<SpriteRenderer>();

    this.velocity = new Vector2(this.driftSpeedMultiplier, 0);
  }
  
  void Update () {
    if(this.facingRight && this.spriteRenderer.flipX) {
      this.spriteRenderer.flipX = false;
    }
    else if(!this.facingRight && !this.spriteRenderer.flipX) {
      this.spriteRenderer.flipX = true;
    }
  }

  void FixedUpdate() {
    Vector2 horizCastSize = new Vector2(Ghost.BarrierDetectionWidth,
                                              this.boxCollider2D.size.y);
    // To avoid detecting ghost's own collider, include buffer distance in center x:
    float bufferDistance = 0.01f;
    float rightCastCenterX = this.boxCollider2D.bounds.max.x + horizCastSize.x / 2 + bufferDistance;
    float leftCastCenterX = this.boxCollider2D.bounds.min.x - horizCastSize.x / 2 - bufferDistance;
    float horizCastCenterY = this.boxCollider2D.bounds.center.y;
    RaycastHit2D rightCastHit = Physics2D.BoxCast(new Vector2(rightCastCenterX, horizCastCenterY),
                                                horizCastSize, 0.0f, Vector2.right,
                                                this.barrierDetectionDistance);
    RaycastHit2D leftCastHit = Physics2D.BoxCast(new Vector2(leftCastCenterX, horizCastCenterY),
                                                horizCastSize, 0.0f, Vector2.left,
                                                this.barrierDetectionDistance);

    if(rightCastHit.collider != null && leftCastHit.collider != null) {
      // Debug.LogFormat("Ghost sees right and left. Not moving horizontally.");
      this.velocity = new Vector2(0, this.velocity.y);
    }
    else if(rightCastHit.collider != null || leftCastHit.collider != null){
      bool hitRight = rightCastHit.collider != null;
      float hitDistance = hitRight ? rightCastHit.distance : leftCastHit.distance;
      float directionMultiplier = hitRight ? 1 : -1;

      if(hitDistance > this.barrierMinDistance) {
        this.velocity -= new Vector2(directionMultiplier * this.barrierSlowdownMultiplier * this.driftSpeedMultiplier, 0);
      }
      else {
        this.velocity = new Vector2(-directionMultiplier * this.driftSpeedMultiplier, this.velocity.y);
      }

      if(this.facingRight == hitRight) {
        this.facingRight = !this.facingRight;
      }
    }

    // Debug.LogFormat("velocity = {0}", this.velocity);
    this.rigidBody2D.MovePosition(this.rigidBody2D.position + this.velocity * Time.deltaTime);
  }
}
