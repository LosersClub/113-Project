using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraBoundsChecker))]
[RequireComponent(typeof(EnemyComponent))]
public class ArcherStationaryController : MonoBehaviour {

  private CameraBoundsChecker cameraBoundsChecker;
  private EnemyComponent enemyComponent;

  void Start () {
    this.cameraBoundsChecker = this.GetComponent<CameraBoundsChecker>();
    this.enemyComponent = this.GetComponent<EnemyComponent>();
  }

  void Update () {
    this.enemyComponent.LookAtTarget();
  }

  public bool InFiringRange() {
    return !this.cameraBoundsChecker.IsOutOfBounds();
  }
}
