﻿using UnityEngine;

public class ColliderDamageDealer : DamageDealer {

  public bool UseBaseCollider = true;
  [SerializeField, ConditionalHide("UseBaseCollider", inverse: true)]
  private Collider2D damageCollider;

  private readonly Collider2D[] hits = new Collider2D[10];
  private ContactFilter2D contactFilter;

  private Collider2D lastHit;
  public override Collider2D LastHit {
    get { return this.lastHit; }
  }

  public void SetCollider(Collider2D collider) {
    this.damageCollider = collider;
    this.contactFilter.layerMask = this.hittableLayers;
  }

  private void Awake() {
    if (this.UseBaseCollider) {
      this.damageCollider = this.GetComponent<Collider2D>();
    }
    this.contactFilter.layerMask = this.hittableLayers;
    this.contactFilter.useLayerMask = true;
    this.contactFilter.useTriggers = true;
  }

  private void OnValidate() {
    this.contactFilter.layerMask = this.hittableLayers;
  }

  private void Update() {
    if (!this.CanDealDamage) {
      return;
    }

    int count = this.damageCollider.OverlapCollider(this.contactFilter, this.hits);
    for (int i = 0; i < count; i++) {
      this.lastHit = this.hits[i];
      this.PerformHit(this.lastHit.GetComponent<DamageTaker>());
    }
  }
}