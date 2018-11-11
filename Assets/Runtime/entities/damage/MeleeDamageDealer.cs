using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MeleeDamageDealer : DamageDealer {

  public Vector2 size;
  public Vector2 offset;
  public LayerMask groundLayer;

  [SerializeField]
  private RayData rayData;

  private readonly RaycastHit2D[] hits = new RaycastHit2D[10];
  private BoxCollider2D startBounds;

  private Collider2D lastHit;
  public override Collider2D LastHit {
    get { return this.lastHit; }
  }
  public Vector2 HitDirection { get; private set; }

  private struct Origins {
    public Vector2 rightSide;
    public Vector2 leftSide;
    public Vector2 topSide;
    public Vector2 botSide;
  }
  private Origins origins;

  private void Awake() {
    this.startBounds = this.GetComponent<BoxCollider2D>();
    this.rayData.Bounds = size;
  }

  private void OnValidate() {
    this.rayData.Bounds = size;
  }

  public void CalculateOrigins() {
    Bounds baseBounds = this.startBounds.bounds;
    baseBounds.Expand(-2f * this.rayData.Inset);

    this.origins.rightSide = new Vector2(baseBounds.max.x,
        this.startBounds.bounds.center.y + offset.y + size.x / 2.0f);
    this.origins.leftSide = new Vector2(baseBounds.min.x,
        this.startBounds.bounds.center.y + offset.y + size.x / 2.0f);
    this.origins.topSide = new Vector2(this.startBounds.bounds.center.x - offset.x - size.x / 2.0f,
        baseBounds.max.y);
    this.origins.botSide = new Vector2(this.startBounds.bounds.center.x - offset.x - size.x / 2.0f,
        baseBounds.min.y);
  }

  private void ComputeHits(Vector2 ray, Vector2 direction, float distance) {
    int count = Physics2D.RaycastNonAlloc(ray, direction, this.hits, distance,
      this.hittableLayers | this.groundLayer);
    this.HitDirection = direction;
    for (int i = 0; i < count; i++) {
      this.lastHit = this.hits[i].collider;
      this.PerformHit(this.lastHit.GetComponent<DamageTaker>());
      if (this.groundLayer == (this.groundLayer | (1 << this.hits[i].transform.gameObject.layer))) {
        Rays.DrawRay(ray, direction, this.hits[i].distance, Color.blue);
        return;
      }
    }
    Rays.DrawRay(ray, direction, size.y, Color.blue);
  }

  public void HorizontalHit(bool right) {
    if (!this.CanDealDamage) {
      return;
    }
    this.CalculateOrigins();

    float distance = this.size.y + this.rayData.Inset;
    Vector2 direction, origin;
    if (right) {
      direction = Vector2.right;
      origin = this.origins.rightSide;
    } else {
      direction = Vector2.left;
      origin = this.origins.leftSide;
    }

    for (int i = 0; i < this.rayData.HorizontalRays; i++) {
      this.ComputeHits(new Vector2(origin.x, origin.y - (i * this.rayData.Spacing.x)),
        direction, distance);
    }
  }

  public void VerticalHit(bool up) {
    if (!this.CanDealDamage) {
      return;
    }
    this.CalculateOrigins();

    float distance = size.y + this.rayData.Inset;
    Vector2 direction, origin;
    if (up) {
      direction = Vector2.up;
      origin = this.origins.topSide;
    } else {
      direction = Vector2.down;
      origin = this.origins.botSide;
    }

    for (int i = 0; i < this.rayData.VerticalRays; i++) {
      this.ComputeHits(new Vector2(origin.x + (i * this.rayData.Spacing.x), origin.y),
        direction, distance);
    }
  }
}