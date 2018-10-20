#define DEBUG_PHYS_RAYS
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public sealed class PhysicsController : MonoBehaviour {

  [SerializeField, Range(2,20)]
  private int horizontalRays = 8;
  [SerializeField, Range(2,20)]
  private int verticalRays = 4;
  [SerializeField, Range(0.001f, 0.3f)]
  private float rayInset = 0.02f;
  [SerializeField]
  private LayerMask groundMask = 0;
  [SerializeField]
  private LayerMask triggerMask = 0;
  [ReadOnly]
  public Vector2 velocity;

  private BoxCollider2D boxCollider;
  private bool grounded = true;
  private Vector2 raySpacing;

  private struct Origins {
    public Vector2 topLeft;
    public Vector2 bottomLeft;
    public Vector2 bottomRight;
  }
  private Origins origins;
  private const float insetPrecision = 0.001f;

  #region Properties
  public bool Grounded { get { return grounded; } }
  public float RayInset {
    get { return this.rayInset; }
    set {
      this.rayInset = value;
      this.RecalculateRaySpacing();
    }
  }
  #endregion

  #region Unity Methods
  private void Awake() {
    this.boxCollider = this.GetComponent<BoxCollider2D>();
    this.RayInset = this.rayInset;

    for (int i = 0; i < 32; i++) {
      // Disable Collisions on all layers except 
      if ((this.triggerMask & 1 << i) == 0) {
        Physics2D.IgnoreLayerCollision(this.gameObject.layer, i);
      }
    }
  }

  private void OnValidate() {
    this.RayInset = rayInset;
  }
  #endregion

  public void Move(Vector2 movement) {
    this.grounded = false;
    this.CreateRayOrigins();

    if (movement.x != 0f) {
      this.CalculateHorizontal(ref movement);
    }
    if (movement.y != 0f) {
      this.CalculateVertical(ref movement);
    }

    this.transform.Translate(movement, Space.World);

    // TODO: Switch to FixedDeltaTime
    if (Time.deltaTime > 0f) {
      this.velocity = movement / Time.deltaTime;
    }
  }

  public void RecalculateRaySpacing() {
    if (this.boxCollider == null) {
      this.boxCollider = this.GetComponent<BoxCollider2D>();
    }

    this.raySpacing = new Vector2(
      (this.boxCollider.size.x * Mathf.Abs(this.transform.localScale.x) -
          (2f * this.rayInset)) / (this.verticalRays - 1),
      (this.boxCollider.size.y * Mathf.Abs(this.transform.localScale.y) -
          (2f * this.rayInset)) / (this.horizontalRays - 1)
    );
  }

  [System.Diagnostics.Conditional("DEBUG_PHYS_RAYS")]
  private void DrawRay(Vector2 start, Vector2 dir, Color color) {
    Debug.DrawRay(start, dir, color);
  }

  private void CreateRayOrigins() {
    Bounds insetBounds = this.boxCollider.bounds;
    insetBounds.Expand(-2f * this.rayInset);

    this.origins.topLeft = new Vector2(insetBounds.min.x, insetBounds.max.y);
    this.origins.bottomLeft = insetBounds.min;
    this.origins.bottomRight = new Vector2(insetBounds.max.x, insetBounds.min.y);
  }

  private void CalculateHorizontal(ref Vector2 movement) {
    bool right = movement.x > 0;
    float distance = Mathf.Abs(movement.x) + this.rayInset;
    Vector2 direction, origin;
    if (right) {
      direction = Vector2.right;
      origin = this.origins.bottomRight;
    } else {
      direction = Vector2.left;
      origin = this.origins.bottomLeft;
    }

    for (int i = 0; i < this.horizontalRays; i++) {
      Vector2 ray = new Vector2(origin.x, origin.y + i * this.raySpacing.y);
      DrawRay(ray, direction * distance, Color.blue);

      RaycastHit2D hit = Physics2D.Raycast(ray, direction, distance, this.groundMask);
      if (hit) {
        // If hit, reduce move distance to that point. Decrease distance since object
        // can no longer reach that distance
        movement.x = hit.point.x - ray.x;
        distance = Mathf.Abs(movement.x);
        // Remove inset from movement
        movement.x += right ? -this.rayInset : this.rayInset;

        if (distance < this.rayInset + insetPrecision) {
          break;
        }
      }
    }
  }

  private void CalculateVertical(ref Vector2 movement) {
    bool up = movement.y > 0;
    float distance = Mathf.Abs(movement.y) + this.rayInset;
    Vector2 direction, origin;
    if (up) {
      direction = Vector2.up;
      origin = this.origins.topLeft;
    } else {
      direction = Vector2.down;
      origin = this.origins.bottomLeft;
    }

    // raycast from x-pos we will be at (horiz before vert)
    origin.x += movement.x;

    for (int i = 0; i < this.verticalRays; i++) {
      Vector2 ray = new Vector2(origin.x + i * this.raySpacing.x, origin.y);
      DrawRay(ray, direction * distance, Color.blue);

      RaycastHit2D hit = Physics2D.Raycast(ray, direction, distance, groundMask);
      if (hit) {
        movement.y = hit.point.y - ray.y;
        distance = Mathf.Abs(movement.y);

        if (up) {
          movement.y -= this.rayInset;
        } else {
          movement.y += this.rayInset;
          this.grounded = true;
        }

        if (distance < this.rayInset + insetPrecision) {
          break;
        }
      }
    }
  }
}