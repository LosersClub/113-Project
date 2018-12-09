using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(SpriteRenderer))]
public class Chalice : MonoBehaviour {

  [SerializeField]
  private GameObject beamEmitterObject;

  private SpriteRenderer spriteRendererComponent;

  private BeamEmitter beamEmitterComponent;

  void Start () {
    Assert.IsNotNull(this.beamEmitterObject);

    this.spriteRendererComponent = this.GetComponent<SpriteRenderer>();

    this.beamEmitterComponent = this.beamEmitterObject.GetComponent<BeamEmitter>();
    Assert.IsNotNull(this.beamEmitterComponent);

    StartCoroutine(FireCoroutine());
  }

  void Update () {
    if(this.beamEmitterComponent.IsFiring) {
      this.spriteRendererComponent.color = Color.blue;
    }
    else {
      this.spriteRendererComponent.color = Color.white;
      this.AimAtPlayer();
    }
  }

  private void AimAtPlayer() {
    Vector2 direction = GameManager.Player.transform.position - this.transform.position;
    direction.Normalize();
    Vector3 directionUpwards = Vector3.Cross(new Vector3(direction.x, direction.y, 0), -Vector3.forward);
    this.transform.rotation = Quaternion.LookRotation(Vector3.forward, directionUpwards);
  }

  private IEnumerator FireCoroutine() {
    while(true) {
      yield return new WaitForSeconds(3);
      this.beamEmitterComponent.Fire();
      yield return new WaitUntil(() => !this.beamEmitterComponent.IsFiring);
    }
  }
}
