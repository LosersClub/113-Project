using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class BeamEmitter : MonoBehaviour {

  private ParticleSystem particleSystemComponent;

  void Start () {
    this.particleSystemComponent = this.GetComponent<ParticleSystem>();
  }

  void Update () {

  }

  public bool IsFiring {
    get {
      return this.particleSystemComponent.isPlaying;
    }
  }

  public void Fire() {
    this.particleSystemComponent.Play();
  }
}
