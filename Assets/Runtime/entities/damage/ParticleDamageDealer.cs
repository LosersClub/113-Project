using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Usage: enable Collision module in ParticleSystem component.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public class ParticleDamageDealer : DamageDealer {

  private ParticleSystem particleSystemComponent;

  private Collider2D lastHit;

  public override Collider2D LastHit {
    get {
      return this.lastHit;
    }
  }

  void Start () {
    this.particleSystemComponent = this.GetComponent<ParticleSystem>();

    // Store CollisionModule in temp variable, to be able to assign to collidesWith below:
    ParticleSystem.CollisionModule collisionModule = this.particleSystemComponent.collision;
    if(collisionModule.collidesWith != this.hittableLayers) {
      Debug.LogWarning("Setting ParticleSystem collidesWith to DamageDealer hittableLayers");
      collisionModule.collidesWith = this.hittableLayers;
    }
  }
  
  void Update () {
    
  }

  void OnParticleCollision(GameObject other) {
    if (!this.CanDealDamage) {
      return;
    }

    this.lastHit = other.GetComponent<Collider2D>();
    this.PerformHit(this.lastHit.GetComponent<DamageTaker>());
  }
}
