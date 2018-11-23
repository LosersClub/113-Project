using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CameraBoundsChecker))]
public class Ghost : MonoBehaviour {

  public float driftSpeedMultiplier = 2.0f;
  public float barrierDetectionDistance = 0.5f;
  public float barrierMinDistance = 0.1f;
  public float barrierSlowdownXMultiplier = 0.1f;
  public float barrierSlowdownYMultiplier = 0.5f;
  public float driftWaveAmplitude = 2.0f;
  public float driftWaveFrequency = 2.0f;
  public float driftWaveYShiftMultiplier = 0.1f;
  public float cameraBoundsBuffer = 2.0f;
  public LayerMask barrierLayerMask = 0;
  public bool initialFacingRight = true;

  private const float BarrierDetectionWidth = 0.05f;

  private BoxCollider2D boxCollider2D;
  private Rigidbody2D rigidBody2D;
  private SpriteRenderer spriteRenderer;
  private CameraBoundsChecker cameraBoundsChecker;

  private Vector2 velocity = Vector2.zero; // will be changed on first FixedUpdate
  private bool facingRight;
  private float driftWavePhase = 0.0f;
  private bool waveShiftUp = true;

  private struct MovementBoundaries {
    private float? rightBarrierDistance, leftBarrierDistance, upBarrierDistance,
                    downBarrierDistance;

    // If near a barrier, that argument should be a float value. Else, should be null.
    public MovementBoundaries(float? rightBarrierDistance, float? leftBarrierDistance,
                              float? upBarrierDistance, float? downBarrierDistance,
                              bool isPastCameraLeft, bool isPastCameraRight,
                              bool isPastCameraBottom, bool isPastCameraTop) {
      this.rightBarrierDistance = rightBarrierDistance;
      this.leftBarrierDistance = leftBarrierDistance;
      this.upBarrierDistance = upBarrierDistance;
      this.downBarrierDistance = downBarrierDistance;

      this.IsPastCameraLeft = isPastCameraLeft;
      this.IsPastCameraRight = isPastCameraRight;
      this.IsPastCameraBottom = isPastCameraBottom;
      this.IsPastCameraTop = isPastCameraTop;
    }

    private static bool NearBarrier(float? barrierDistance) {
      return barrierDistance != null;
    }
    private static float BarrierDistanceHelper(float? barrierDistance) {
      if(!barrierDistance.HasValue) {
          throw new System.InvalidOperationException("No barrier distance when not near barrier");
      }
      return barrierDistance.Value;
    }

    public bool NearBarrierRight {
      get { return MovementBoundaries.NearBarrier(this.rightBarrierDistance); }
    }
    public bool NearBarrierLeft {
      get { return MovementBoundaries.NearBarrier(this.leftBarrierDistance); }
    }
    public bool NearBarrierUp {
      get { return MovementBoundaries.NearBarrier(this.upBarrierDistance); }
    }
    public bool NearBarrierDown {
      get { return MovementBoundaries.NearBarrier(this.downBarrierDistance); }
    }

    public float BarrierRightDistance {
      get { return MovementBoundaries.BarrierDistanceHelper(this.rightBarrierDistance); }
    }
    public float BarrierLeftDistance {
      get { return MovementBoundaries.BarrierDistanceHelper(this.leftBarrierDistance); }
    }
    public float BarrierUpDistance {
      get { return MovementBoundaries.BarrierDistanceHelper(this.upBarrierDistance); }
    }
    public float BarrierDownDistance {
      get { return MovementBoundaries.BarrierDistanceHelper(this.downBarrierDistance); }
    }

    public bool IsPastCameraLeft { get; }
    public bool IsPastCameraRight { get; }
    public bool IsPastCameraBottom { get; }
    public bool IsPastCameraTop { get; }
  }

  void Start () {
    Assert.IsTrue(this.barrierMinDistance < this.barrierDetectionDistance);

    this.boxCollider2D = this.GetComponent<BoxCollider2D>();
    this.rigidBody2D = this.GetComponent<Rigidbody2D>();
    this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    this.cameraBoundsChecker = this.GetComponent<CameraBoundsChecker>();

    this.facingRight = initialFacingRight;
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
    MovementBoundaries moveBoundaries = this.CalculateMovementBoundaries();
    this.UpdateVelocityX(moveBoundaries);
    this.UpdateVelocityY(moveBoundaries);

    if(this.velocity.x > 0) {
      this.facingRight = true;
    }
    else if(this.velocity.x < 0) {
      this.facingRight = false;
    }
    // If velocity.x == 0, don't change direction.

    this.rigidBody2D.MovePosition(this.rigidBody2D.position + this.velocity * Time.deltaTime);
  }

  private void UpdateVelocityX(MovementBoundaries moveBoundaries) {
    float newVelocityX = this.velocity.x;

    if(moveBoundaries.NearBarrierRight && moveBoundaries.NearBarrierLeft) {
      newVelocityX = 0;
    }
    else if(moveBoundaries.NearBarrierRight || moveBoundaries.NearBarrierLeft) {
      float hitDistance = moveBoundaries.NearBarrierRight ? moveBoundaries.BarrierRightDistance : moveBoundaries.BarrierLeftDistance;
      float directionMultiplier = moveBoundaries.NearBarrierRight ? 1 : -1;

      if(hitDistance > this.barrierMinDistance) {
        newVelocityX -= directionMultiplier * this.barrierSlowdownXMultiplier * this.driftSpeedMultiplier;
      }
      else {
        newVelocityX = -directionMultiplier * this.driftSpeedMultiplier;
      }
    }
    else if(moveBoundaries.IsPastCameraLeft || moveBoundaries.IsPastCameraRight) {
      newVelocityX = (moveBoundaries.IsPastCameraLeft ? 1 : -1) * this.driftSpeedMultiplier;
    }
    else if(Mathf.Abs(this.velocity.x) < 0.01) { // if almost stopped moving
      // Start moving again:
      newVelocityX = (this.facingRight ? 1 : -1) * this.driftSpeedMultiplier;
    }

    this.velocity = new Vector2(newVelocityX, this.velocity.y);
  }

  private void UpdateVelocityY(MovementBoundaries moveBoundaries) {
    float newVelocityY = this.velocity.y;

    if(moveBoundaries.NearBarrierUp && moveBoundaries.NearBarrierDown) {
      newVelocityY = 0;
    }
    else {
      float velocityYOffset = 0;
      float waveShiftYVelocity = 0;

      if(moveBoundaries.NearBarrierUp || moveBoundaries.NearBarrierDown) {
        this.waveShiftUp = moveBoundaries.NearBarrierDown;

        float hitDistance = moveBoundaries.NearBarrierUp ? moveBoundaries.BarrierUpDistance : moveBoundaries.BarrierDownDistance;
        float directionMultiplier = moveBoundaries.NearBarrierUp ? -1 : 1;

        if(hitDistance > this.barrierMinDistance) {
          velocityYOffset = directionMultiplier * this.barrierSlowdownYMultiplier * this.driftSpeedMultiplier;
        }
        else {
          this.driftWavePhase = -2 * Mathf.PI * this.driftWaveFrequency * Time.time + (moveBoundaries.NearBarrierUp ? 1.05f : 0.05f) * Mathf.PI;
        }
      }
      else if(moveBoundaries.IsPastCameraTop || moveBoundaries.IsPastCameraBottom) {
        this.waveShiftUp = moveBoundaries.IsPastCameraBottom;

        this.driftWavePhase = -2 * Mathf.PI * this.driftWaveFrequency * Time.time + (moveBoundaries.IsPastCameraTop ? 1.05f : 0.05f) * Mathf.PI;
      }
      else {
        waveShiftYVelocity = (this.waveShiftUp ? 1 : -1) * this.driftWaveYShiftMultiplier * this.driftSpeedMultiplier;
      }

      // Mod argument to Mathf.Sin by 2*pi, since Mathf.Sin fails for large values:
      float newVelocityYSin = Mathf.Sin((2 * Mathf.PI * this.driftWaveFrequency * Time.time + this.driftWavePhase) % (2 * Mathf.PI));
      newVelocityY = this.driftWaveAmplitude * newVelocityYSin + velocityYOffset + waveShiftYVelocity;
    }

    this.velocity = new Vector2(this.velocity.x, newVelocityY);
  }

  private MovementBoundaries CalculateMovementBoundaries() {
    Vector2 horizCastSize = new Vector2(Ghost.BarrierDetectionWidth, this.boxCollider2D.size.y);
    Vector2 vertCastSize = new Vector2(this.boxCollider2D.size.x, Ghost.BarrierDetectionWidth);
    float horizCastCenterY = this.boxCollider2D.bounds.center.y;
    float vertCastCenterX = this.boxCollider2D.bounds.center.x;
    // To avoid detecting ghost's own collider, include buffer distance in cast start:
    float bufferDistance = 0.01f;

    float? rightCastDistance = null;
    float? leftCastDistance = null;
    if(this.velocity.x > 0) {
      float rightCastCenterX = this.boxCollider2D.bounds.max.x + horizCastSize.x / 2 + bufferDistance;
      RaycastHit2D rightCastHit = Physics2D.BoxCast(new Vector2(rightCastCenterX, horizCastCenterY),
                                                  horizCastSize, 0.0f, Vector2.right,
                                                  this.barrierDetectionDistance, this.barrierLayerMask);
      if(rightCastHit.collider != null) {
        rightCastDistance = (float?)rightCastHit.distance;
      }
    }
    else {
      float leftCastCenterX = this.boxCollider2D.bounds.min.x - horizCastSize.x / 2 - bufferDistance;
      RaycastHit2D leftCastHit = Physics2D.BoxCast(new Vector2(leftCastCenterX, horizCastCenterY),
                                                  horizCastSize, 0.0f, Vector2.left,
                                                  this.barrierDetectionDistance, this.barrierLayerMask);
      if(leftCastHit.collider != null) {
        leftCastDistance = (float?)leftCastHit.distance;
      }
    }

    float? upCastDistance = null;
    float? downCastDistance = null;
    if(this.velocity.y > 0) {
      float upCastCenterY = this.boxCollider2D.bounds.max.y + vertCastSize.y / 2 + bufferDistance;
      RaycastHit2D upCastHit = Physics2D.BoxCast(new Vector2(vertCastCenterX, upCastCenterY),
                                                  vertCastSize, 0.0f, Vector2.up,
                                                  this.barrierDetectionDistance, this.barrierLayerMask);
      if(upCastHit.collider != null) {
        upCastDistance = (float?)upCastHit.distance;
      }
    }
    else {
      float downCastCenterY = this.boxCollider2D.bounds.min.y - vertCastSize.y / 2 - bufferDistance;
      RaycastHit2D downCastHit = Physics2D.BoxCast(new Vector2(vertCastCenterX, downCastCenterY),
                                                    vertCastSize, 0.0f, Vector2.down,
                                                    this.barrierDetectionDistance, this.barrierLayerMask);
      if(downCastHit.collider != null) {
        downCastDistance = (float?)downCastHit.distance;
      }
    }

    return new MovementBoundaries(rightCastDistance, leftCastDistance, upCastDistance, downCastDistance,
                                  this.cameraBoundsChecker.IsPastLeft(this.cameraBoundsBuffer),
                                  this.cameraBoundsChecker.IsPastRight(this.cameraBoundsBuffer),
                                  this.cameraBoundsChecker.IsPastBottom(this.cameraBoundsBuffer),
                                  this.cameraBoundsChecker.IsPastTop(this.cameraBoundsBuffer));
  }
}
