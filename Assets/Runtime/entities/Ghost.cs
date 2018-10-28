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
  public float driftWaveAmplitude = 2.0f;
  public float driftWaveFrequency = 2.0f;

  private const float BarrierDetectionWidth = 0.05f;

  private BoxCollider2D boxCollider2D;
  private Rigidbody2D rigidBody2D;
  private SpriteRenderer spriteRenderer;

  private Vector2 velocity;
  private bool facingRight = true;
  private float driftWaveTimeOffset = 0.0f;
  private float driftWaveAmplitudeMultiplier = 1.0f;

  void Start () {
    Assert.IsTrue(barrierMinDistance < barrierDetectionDistance);

    this.boxCollider2D = this.GetComponent<BoxCollider2D>();
    this.rigidBody2D = this.GetComponent<Rigidbody2D>();
    this.spriteRenderer = this.GetComponent<SpriteRenderer>();

    this.velocity = new Vector2(this.driftSpeedMultiplier, this.driftSpeedMultiplier);
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
    Vector2 horizCastSize = new Vector2(Ghost.BarrierDetectionWidth, this.boxCollider2D.size.y);
    Vector2 vertCastSize = new Vector2(this.boxCollider2D.size.x, Ghost.BarrierDetectionWidth);
    // To avoid detecting ghost's own collider, include buffer distance in center x:
    float bufferDistance = 0.01f;
    float rightCastCenterX = this.boxCollider2D.bounds.max.x + horizCastSize.x / 2 + bufferDistance;
    float leftCastCenterX = this.boxCollider2D.bounds.min.x - horizCastSize.x / 2 - bufferDistance;
    float upCastCenterY = this.boxCollider2D.bounds.max.y + vertCastSize.y / 2 + bufferDistance;
    float downCastCenterY = this.boxCollider2D.bounds.min.y - vertCastSize.y / 2 - bufferDistance;
    float horizCastCenterY = this.boxCollider2D.bounds.center.y;
    float vertCastCenterX = this.boxCollider2D.bounds.center.x;
    RaycastHit2D rightCastHit = Physics2D.BoxCast(new Vector2(rightCastCenterX, horizCastCenterY),
                                                horizCastSize, 0.0f, Vector2.right,
                                                this.barrierDetectionDistance);
    RaycastHit2D leftCastHit = Physics2D.BoxCast(new Vector2(leftCastCenterX, horizCastCenterY),
                                                horizCastSize, 0.0f, Vector2.left,
                                                this.barrierDetectionDistance);
    RaycastHit2D upCastHit = Physics2D.BoxCast(new Vector2(vertCastCenterX, upCastCenterY),
                                                vertCastSize, 0.0f, Vector2.up,
                                                this.barrierDetectionDistance);
    RaycastHit2D downCastHit = Physics2D.BoxCast(new Vector2(vertCastCenterX, downCastCenterY),
                                                  vertCastSize, 0.0f, Vector2.down,
                                                  this.barrierDetectionDistance);

    if(rightCastHit.collider != null && leftCastHit.collider != null) {
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

    if(upCastHit.collider != null && downCastHit.collider != null) {
      this.velocity = new Vector2(this.velocity.x, 0);
    }
    else {
      float yVelocityOffset = 0;
      if(upCastHit.collider != null || downCastHit.collider != null) {
        bool hitUp = upCastHit.collider != null;
        float hitDistance = hitUp ? upCastHit.distance : downCastHit.distance;
        float directionMultiplier = hitUp ? 1 : -1;

        if((this.velocity.y > 0) == hitUp) {
          if(hitDistance > this.barrierMinDistance) {
            yVelocityOffset = -directionMultiplier * this.barrierSlowdownMultiplier * this.driftSpeedMultiplier;
          }
          else {
            this.driftWaveTimeOffset = -Time.time + Mathf.Asin(0.1f);
            this.driftWaveAmplitudeMultiplier = hitUp ? -1 : 1;
          }
        }
      }

      float yVelocitySin = Mathf.Sin(this.driftWaveFrequency * ((Time.time + this.driftWaveTimeOffset) % (2 * Mathf.PI)));
      this.velocity = new Vector2(this.velocity.x, yVelocityOffset + this.driftWaveAmplitude * this.driftWaveAmplitudeMultiplier * yVelocitySin);
    }

    this.rigidBody2D.MovePosition(this.rigidBody2D.position + this.velocity * Time.deltaTime);
  }
}
