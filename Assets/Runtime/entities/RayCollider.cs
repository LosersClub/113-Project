using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RayCollider : MonoBehaviour {
  [SerializeField, Range(2, 20)]
  private int horizonalRays = 8;
  [SerializeField, Range(2, 20)]
  private int verticalRays = 4;
  [SerializeField, Range(-0.3f, 0.3f)]
  private float rayInset = 0.02f;

  private BoxCollider2D boxCollider;

  public Vector2 Spacing { get; private set; }
  public float Inset {
    get { return rayInset; }
    set {
      this.rayInset = value;
      this.RecalculateRaySpacing();
    }
  }
  public int HorizontalRays { get { return this.horizonalRays; } }
  public int VerticalRays { get { return this.verticalRays; } }

  public struct RaycastOrigin {
    public Vector2 topLeft;
    public Vector2 bottomLeft;
    public Vector2 bottomRight;
  }
  private RaycastOrigin origins;
  public RaycastOrigin Origins { get { return origins; } }

  public virtual void Awake() {
    this.boxCollider = this.GetComponent<BoxCollider2D>();
    this.Inset = this.rayInset;
  }

  public virtual void OnValidate() {
    this.Inset = this.rayInset;
  }

  public void RecalculateRaySpacing() {
    if (this.boxCollider == null) {
      this.boxCollider = this.GetComponent<BoxCollider2D>();
    }

    this.Spacing = new Vector2(
      (this.boxCollider.size.x * Mathf.Abs(this.transform.localScale.x) -
          (2f * this.rayInset)) / (this.verticalRays - 1),
      (this.boxCollider.size.y * Mathf.Abs(this.transform.localScale.y) -
          (2f * this.rayInset)) / (this.horizonalRays - 1)
    );
  }

  public void UpdateRayOrigins() {
    Bounds insetBounds = this.boxCollider.bounds;
    insetBounds.Expand(-2f * this.rayInset);

    this.origins.topLeft = new Vector2(insetBounds.min.x, insetBounds.max.y);
    this.origins.bottomLeft = insetBounds.min;
    this.origins.bottomRight = new Vector2(insetBounds.max.x, insetBounds.min.y);
  }
}