#define DEBUG_PHYS_RAYS
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public sealed class PhysicsController : MonoBehaviour {

  [ReadOnly]
  public Vector2 velocity;

  [Header("Ray Settings")]
  [SerializeField, Range(2,20)]
  private int horizontalRays = 8;
  [SerializeField, Range(2,20)]
  private int verticalRays = 4;
  [SerializeField, Range(0.001f, 0.3f)]
  private float rayInset = 0.02f;

  [Header("Layer Masks")]
  [SerializeField]
  private LayerMask groundMask = 0;
  [SerializeField]
  private LayerMask platformMask = 0;
  [SerializeField]
  private LayerMask triggerMask = 0;

  private BoxCollider2D boxCollider;
  private Vector2 raySpacing;
  private bool ignorePlatforms = false;

  public struct CollisionState {
    public bool Left { get; set; }
    public bool Right { get; set; }
    public bool Above { get; set; }
    public bool Below { get; set; }

    public bool Collision {
      get { return Left || Right || Above || Below; }
    }

    public void Reset() {
      this.Left = false;
      this.Right = false;
      this.Above = false;
      this.Below = false;
    }
  }
  private CollisionState state;

  private struct Origins {
    public Vector2 topLeft;
    public Vector2 bottomLeft;
    public Vector2 bottomRight;
  }
  private Origins origins;
  private const float insetPrecision = 0.001f;

  #region Properties
  public CollisionState Collision { get { return this.state; } }
  public bool Grounded { get { return this.state.Below; } }
  public bool IgnorePlatforms {
    get { return this.ignorePlatforms; }
    set { this.ignorePlatforms = value; }
  }
  
  public float RayInset {
    get { return this.rayInset; }
    set {
      this.rayInset = value;
      this.RecalculateRaySpacing();
    }
  }
  public int VerticalRays {
    get { return this.verticalRays; }
  }
  public Vector2 RaySpacing {
    get { return this.raySpacing; }
  }

  public LayerMask Ground {
    get { return this.groundMask; }
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
    this.state.Reset();
    this.CreateRayOrigins();

    if (movement.x != 0f) {
      this.CalculateHorizontal(ref movement);
    }
    if (movement.y != 0f) {
      this.CalculateVertical(ref movement);
    }

    this.transform.Translate(movement, Space.World);

    if (Time.deltaTime > 0f) {
      this.velocity = movement / Time.deltaTime;
    }
    this.ignorePlatforms = false;
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
        if (right) {
          movement.x -= this.rayInset;
          this.state.Right = true;
        } else {
          movement.x += this.rayInset;
          this.state.Left = true;
        }

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
    LayerMask mask = groundMask;
    if (!up && !ignorePlatforms) {
      mask |= platformMask;
    }
    for (int i = 0; i < this.verticalRays; i++) {
      Vector2 ray = new Vector2(origin.x + i * this.raySpacing.x, origin.y);
      DrawRay(ray, direction * distance, Color.blue);

      RaycastHit2D hit = Physics2D.Raycast(ray, direction, distance, mask);
      if (hit) {
        movement.y = hit.point.y - ray.y;
        distance = Mathf.Abs(movement.y);

        if (up) {
          movement.y -= this.rayInset;
          this.state.Above = true;
        } else {
          movement.y += this.rayInset;
          this.state.Below = true;
        }

        if (distance < this.rayInset + insetPrecision) {
          break;
        }
      }
    }
  }
}