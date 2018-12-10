using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class BeamEmitter : MonoBehaviour {

  [SerializeField]
  private float fireSpeed = 20f;

  private ParticleSystem particleSystemComponent;

  void Start () {
    this.particleSystemComponent = this.GetComponent<ParticleSystem>();
  }

  void Update () {

  }

  public bool IsFiring {
    get {
      return this.particleSystemComponent.isEmitting;
    }
  }

  public void Fire(Vector2 direction) {
    direction = direction.normalized * this.fireSpeed;
    ParticleSystem.VelocityOverLifetimeModule velocityModule = this.particleSystemComponent.velocityOverLifetime;
    velocityModule.xMultiplier = direction.x;
    velocityModule.yMultiplier = direction.y;
    this.particleSystemComponent.Play();
  }
}
