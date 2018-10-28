//#define DEBUG_PHYS_RAYS
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour {
  [Header("Run Settings")]
  public float gravity = -25f;
  public float runSpeed = 8f;
  public float crouchSpeed = 0.8f;
  public float groundDamping = 20f;

  [Header("Jump Settings")]
  public float airDamping = 10f;
  public float minJumpHeight = 1f;
  public float maxJumpTime = 0.35f;

  [Header("Dash Settings")]
  public float dashSpeed = 100f;
  public float dashDistance = 3f;
  public float dashCooldown = 1f;

  [Header("Checker Rays")]
  public float standSize = 1f;

  [Header("Collider stuff")]
  public Vector2 playerOffset = new Vector2(0f, -0.06f);
  public Vector2 playerSize = new Vector2(0.625f, 1.87f);
  public Vector2 crouchOffset = new Vector2(0f, -0.4f);
  public Vector2 crouchSize = new Vector2(0.625f, 1.17f);

  private MovementController physics;
  private Animator animator;
  private new BoxCollider2D collider;
  private float normalizedMovement;
  private bool facingRight = true;

  private bool crouching = false;
  private bool prevCrouching = false;

  private bool jumping = false;
  private float jumpCounter = 0;

  private bool canDash = true;
  private bool dashing = false;

  private void Awake() {
    this.physics = this.GetComponent<MovementController>();
    this.animator = this.GetComponent<Animator>();
    this.collider = this.GetComponent<BoxCollider2D>();
  }

  private void Update() {
    float targetSpeed = normalizedMovement * runSpeed;
    if (this.crouching) {
      this.animator.SetLayerWeight(1, 1);
      targetSpeed *= crouchSpeed;
    } else {
      this.animator.SetLayerWeight(1, 0);
    }

    if (this.prevCrouching != this.crouching) {
      if (this.crouching) {
        this.collider.offset = this.crouchOffset;
        this.collider.size = this.crouchSize;
      } else {
        this.collider.offset = this.playerOffset;
        this.collider.size = this.playerSize;
      }
      this.physics.RecalculateRaySpacing();
    }

    if (!dashing) {
      this.physics.Velocity.x = Mathf.Lerp(this.physics.Velocity.x, targetSpeed,
        Time.deltaTime * (this.physics.Grounded ? this.groundDamping : this.airDamping));
    }

    if (normalizedMovement > 0) {
      this.transform.eulerAngles = Vector3.zero;
      this.facingRight = true;
    } else if (normalizedMovement < 0) {
      this.transform.eulerAngles = new Vector3(0, 180, 0);
      this.facingRight = false;
    }
    
    if (this.jumping && this.crouching) {
      this.physics.IgnorePlatforms = true;
    }
    if ((this.jumpCounter > 0 && this.physics.Collision.Above) ||
         this.jumpCounter == this.maxJumpTime && this.crouching) {
      this.jumpCounter = 0;
    } else if (this.jumpCounter > 0) {
      this.physics.Velocity.y = Mathf.Sqrt(2f * minJumpHeight * -gravity);
    }

    if (!this.dashing) {
      this.physics.Velocity.y += gravity * Time.deltaTime;
    }

    this.physics.Move(this.physics.Velocity * Time.deltaTime);
    this.animator.SetFloat("horizontalSpeed", Mathf.Abs(this.normalizedMovement));
    this.animator.SetFloat("verticalSpeed", this.physics.Velocity.y);
    this.animator.SetBool("dashing", this.dashing);

    this.normalizedMovement = 0;
    if (!this.jumping) {
      this.jumpCounter = 0;
    }
    this.jumping = false;
    this.prevCrouching = this.crouching;
    if (this.crouching) {
      this.crouching = !this.SpaceToStand();
    }
  }

  private bool SpaceToStand() {
    Bounds inset = this.collider.bounds;
    //inset.Expand(-2f * this.physics.RayInset);
    Vector2 origin = new Vector2(inset.min.x + this.physics.Inset, inset.max.y - this.physics.Inset);
    Vector2 ray;
    for (int i = 0; i < this.physics.VerticalRays; i++) {
      ray = new Vector2(origin.x + i * this.physics.Spacing.x, origin.y);
      Rays.DrawRay(ray, Vector2.up, standSize, Color.blue);
      if (Physics2D.RaycastNonAlloc(ray, Vector2.up, Rays.singleHit, standSize, this.physics.Ground) > 0) {
        return false;
      }
    }
    return true;
  }

  public void Move(float x, float y) {
    if (x > 0.3f) {
      this.normalizedMovement += 1.0f;
    } else if (x < -0.3f) {
      this.normalizedMovement -= 1.0f;
    }

    if (y < -0.3f) {
      this.crouching = true;
    }
  }

  public void Jump() {
    this.jumping = true;
    if (this.physics.Grounded) {
      this.jumpCounter = this.maxJumpTime;
    } else if (this.jumpCounter > 0) {
      this.jumpCounter -= Time.deltaTime;
    }
  }

  public void Dash() {
    if (this.canDash) {
      this.StartCoroutine(this.DashCoroutine());
    }
  }

  private IEnumerator DashCoroutine() {
    float duration = this.dashDistance / this.dashSpeed;
    float time = 0f;
    this.canDash = false;
    this.dashing = true;
    while (duration > time) {
      time += Time.deltaTime;
      this.physics.Velocity.x = this.dashSpeed;
      if (!this.facingRight) {
        this.physics.Velocity.x *= -1;
      }
      yield return 0;
    }
    this.dashing = false;
    yield return new WaitForSeconds(this.dashCooldown);
    canDash = true;
  }
}
