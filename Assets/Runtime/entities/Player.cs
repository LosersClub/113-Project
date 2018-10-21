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

  private PhysicsController physics;
  private Animator animator;
  private float normalizedMovement;
  private bool facingRight = true;

  private bool crouching = false;
  private bool jumping = false;
  private float jumpCounter = 0;

  private bool canDash = true;
  private bool dashing = false;

  private void Awake() {
    this.physics = this.GetComponent<PhysicsController>();
    this.animator = this.GetComponent<Animator>();
  }

  private void Update() {
    this.animator.SetLayerWeight(1, this.crouching ? 1 : 0);

    if (!dashing) {
      float targetSpeed = normalizedMovement * runSpeed * (this.crouching ? crouchSpeed : 1.0f);
      this.physics.velocity.x = Mathf.Lerp(this.physics.velocity.x, targetSpeed,
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
      this.physics.velocity.y = Mathf.Sqrt(2f * minJumpHeight * -gravity);
    }

    if (!this.dashing) {
      this.physics.velocity.y += gravity * Time.deltaTime;
    }

    this.physics.Move(this.physics.velocity * Time.deltaTime);
    this.animator.SetFloat("horizontalSpeed", Mathf.Abs(this.normalizedMovement));
    this.animator.SetFloat("verticalSpeed", this.physics.velocity.y);

    this.normalizedMovement = 0;
    if (!this.jumping) {
      this.jumpCounter = 0;
    }
    this.jumping = false;
    this.crouching = false;
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
      this.physics.velocity.x = this.dashSpeed;
      if (!this.facingRight) {
        this.physics.velocity.x *= -1;
      }
      yield return 0;
    }
    this.dashing = false;
    yield return new WaitForSeconds(this.dashCooldown);
    canDash = true;
  }
}
