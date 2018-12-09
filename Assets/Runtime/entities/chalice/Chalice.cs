using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Chalice : MonoBehaviour {

  [SerializeField]
  private GameObject beamEmitterObject;

  private BeamEmitter beamEmitterComponent;

  void Start () {
    Assert.IsNotNull(this.beamEmitterObject);

    this.beamEmitterComponent = this.beamEmitterObject.GetComponent<BeamEmitter>();
    Assert.IsNotNull(this.beamEmitterComponent);

    StartCoroutine(FireCoroutine());
  }

  void Update () {

  }

  private IEnumerator FireCoroutine() {
    while(true) {
      yield return new WaitForSeconds(3);
      this.beamEmitterComponent.Fire();
      yield return new WaitUntil(() => !this.beamEmitterComponent.IsFiring);
    }
  }
}
