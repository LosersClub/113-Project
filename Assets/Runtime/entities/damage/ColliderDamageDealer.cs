using UnityEngine;

public class ColliderDamageDealer : DamageDealer {

  public bool UseBaseCollider = true;
  [SerializeField, ConditionalHide("UseBaseCollider", inverse: true)]
  private BoxCollider2D damageCollider;

  private readonly Collider2D[] hits = new Collider2D[10];
  private ContactFilter2D contactFilter;

  private Collider2D lastHit;
  public override Collider2D LastHit {
    get { return this.lastHit; }
  }

  private void Awake() {
    if (this.UseBaseCollider) {
      this.damageCollider = this.GetComponent<BoxCollider2D>();
    }
    this.contactFilter.layerMask = this.hittableLayers;
    this.contactFilter.useLayerMask = true;
    this.contactFilter.useTriggers = true;
  }

  private void Update() {
    if (!this.CanDealDamage) {
      return;
    }

    int count = Physics2D.OverlapCollider(this.damageCollider, this.contactFilter, this.hits);
    for (int i = 0; i < count; i++) {
      this.lastHit = this.hits[i];
      this.PerformHit(this.lastHit.GetComponent<DamageTaker>());
    }
  }
}