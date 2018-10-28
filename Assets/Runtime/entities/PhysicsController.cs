using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public sealed class PhysicsController : MonoBehaviour {

  [ReadOnly]
  public Vector2 velocity;
  public RayCollider rayCollider;

  [Header("Layer Masks")]
  [SerializeField]
  private LayerMask groundMask = 0;
  [SerializeField]
  private LayerMask platformMask = 0;

  private bool ignorePlatforms = false;
  private BoxCollider2D boxCollider;
  // Prevent heap allocations
  private readonly RaycastHit2D[] hit = new RaycastHit2D[1];

  public float Inset {
    get { return this.rayCollider.Inset; }
  }
  public Vector2 Spacing {
    get { return this.rayCollider.Spacing; }
  }

  public int HorizontalRays {
    get { return this.rayCollider.HorizontalRays; }
  }
  public int VerticalRays {
    get { return this.rayCollider.VerticalRays; }
  }

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

  private void Awake() {
    this.boxCollider = this.GetComponent<BoxCollider2D>();
    this.rayCollider.Bounds = this.boxCollider.size;
  }

  #region Properties
  public CollisionState Collision { get { return this.state; } }
  public bool Grounded { get { return this.state.Below; } }
  public bool IgnorePlatforms {
    get { return this.ignorePlatforms; }
    set { this.ignorePlatforms = value; }
  }

  public LayerMask Ground {
    get { return this.groundMask; }
  }
  #endregion

  public void RecalculateRaySpacing() {
    this.rayCollider.Bounds = this.boxCollider.size;
  }

  public void Move(Vector2 movement) {
    this.rayCollider.UpdateOrigins(this.boxCollider);
    this.state.Reset();

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

  private void CalculateHorizontal(ref Vector2 movement) {
    bool right = movement.x > 0;
    float distance = Mathf.Abs(movement.x) + this.rayCollider.Inset;
    Vector2 direction, origin;
    if (right) {
      direction = Vector2.right;
      origin = this.rayCollider.origins.bottomRight;
    } else {
      direction = Vector2.left;
      origin = this.rayCollider.origins.bottomLeft;
    }

    for (int i = 0; i < this.HorizontalRays; i++) {
      Vector2 ray = new Vector2(origin.x, origin.y + i * this.Spacing.y);
      Rays.DrawRay(ray, direction, distance, Color.blue);

      if (Physics2D.RaycastNonAlloc(ray, direction, this.hit, distance, this.groundMask) > 0) {
        movement.x = hit[0].point.x - ray.x;
        distance = Mathf.Abs(movement.x);
        if (right) {
          movement.x -= this.Inset;
          this.state.Right = true;
        } else {
          movement.x += this.Inset;
          this.state.Left = true;
        }

        if (distance < this.Inset + Rays.Precision) {
          break;
        }
      }
    }
  }

  private void CalculateVertical(ref Vector2 movement) {
    bool up = movement.y > 0;
    float distance = Mathf.Abs(movement.y) + this.Inset;
    Vector2 direction, origin;
    if (up) {
      direction = Vector2.up;
      origin = this.rayCollider.origins.topLeft;
    } else {
      direction = Vector2.down;
      origin = this.rayCollider.origins.bottomLeft;
    }

    // raycast from x-pos we will be at (horiz before vert)
    origin.x += movement.x;
    LayerMask mask = groundMask;
    if (!up && !ignorePlatforms) {
      mask |= platformMask;
    }

    for (int i = 0; i < this.VerticalRays; i++) {
      Vector2 ray = new Vector2(origin.x + i * this.Spacing.x, origin.y);
      Rays.DrawRay(ray, direction, distance, Color.blue);

      if (Physics2D.RaycastNonAlloc(ray, direction, this.hit, distance, mask) > 0) {
        movement.y = hit[0].point.y - ray.y;
        distance = Mathf.Abs(movement.y);
        if (up) {
          movement.y -= this.Inset;
          this.state.Above = true;
        } else {
          movement.y += this.Inset;
          this.state.Below = true;
        }

        if (distance < this.Inset + Rays.Precision) {
          break;
        }
      }
    }
  }
}