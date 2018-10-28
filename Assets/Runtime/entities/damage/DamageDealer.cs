using UnityEngine;

public abstract class DamageDealer : MonoBehaviour {
  [SerializeField]
  protected float damage = 1.0f;

  [SerializeField]
  protected bool disableDamageAfterHit;
  [SerializeField]
  protected bool ignoreInvulnerability;
  [SerializeField]
  protected LayerMask hittableLayers;

  [SerializeField]
  protected DamageEvent OnDamageHit;
  [SerializeField]
  protected NonDamageEvent OnNoDamageHit;

  public abstract Collider2D LastHit { get; }
  private bool canDealDamage = true;
  public bool CanDealDamage {
    get { return canDealDamage; }
    set { this.canDealDamage = value; }
  }

  public float Damage { get { return this.damage; } }
  public bool IgnoreInvulnerability { get { return this.ignoreInvulnerability; } }

  protected virtual void PerformHit(DamageTaker objHit) {
    if (objHit) {
      this.OnDamageHit.Invoke(this, objHit);
      objHit.TakeDamage(this, this.ignoreInvulnerability);
      if (this.disableDamageAfterHit) {
        this.CanDealDamage = false;
      }
    } else {
      this.OnNoDamageHit.Invoke(this);
    }
  }
}