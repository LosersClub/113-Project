using UnityEngine;

public class Player : MonoBehaviour {

  public float gravity = -25f;
  public float runSpeed = 8f;
  public float groundDamping = 20f;

  public float airDamping = 10f;
  public float minJumpHeight = 1f;
  public float maxJumpTime = 0.35f;

  private PhysicsController physics;
  private float normalizedMovement;

  private bool crouching = false;
  private bool jumping = false;
  private float jumpCounter = 0;

  private void Awake() {
    this.physics = this.GetComponent<PhysicsController>();
  }

  private void Update() {
    this.physics.velocity.x = Mathf.Lerp(this.physics.velocity.x, normalizedMovement * runSpeed,
      Time.deltaTime * (this.physics.Grounded ? this.groundDamping : this.airDamping));

    if (normalizedMovement > 0) {
      this.transform.eulerAngles = Vector3.zero;
    } else if (normalizedMovement < 0) {
      this.transform.eulerAngles = new Vector3(0, 180, 0);
    }

    if (this.jumpCounter > 0) {
      this.physics.velocity.y = Mathf.Sqrt(2f * minJumpHeight * -gravity);
    }

    this.physics.velocity.y += gravity * Time.deltaTime;
    this.physics.Move(this.physics.velocity * Time.deltaTime);

    this.normalizedMovement = 0;
    if (!this.jumping) {
      this.jumpCounter = 0;
    }
    this.jumping = false;
    this.crouching = false;
  }

  public void Move(float x, float y) {
    this.normalizedMovement += x;

    if (y < 0) {
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

  private Collider2D GetCurrentGroundCollider() {
    return null;
  }

  private void DropDown() {
    // Note: allows multiple calls as drop key is held down, for case where
    // drop key starts being held down before touching platform.
    Collider2D groundCollider = GetCurrentGroundCollider();
    if(groundCollider != null) {
      var platform = groundCollider.gameObject.GetComponent<Platform>();
      if(platform != null) {
        platform.AllowFallThrough(this.gameObject);
      }
    }
  }
}
