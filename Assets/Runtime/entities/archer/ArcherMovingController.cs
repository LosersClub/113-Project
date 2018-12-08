using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyComponent))]
[RequireComponent(typeof(ChaseComponent))]
public class ArcherMovingController : MonoBehaviour, IArcherFireChecker {

  private EnemyComponent enemyComponent;
  private ChaseComponent chaseComponent;

  void Start () {
    this.enemyComponent = this.GetComponent<EnemyComponent>();
    this.chaseComponent = this.GetComponent<ChaseComponent>();
  }
  
  void Update () {
    if(this.enemyComponent.inAction) {
      this.enemyComponent.LookAtTarget();
    }
  }

  public bool CanFire() {
    return this.chaseComponent.WithinStopDistance();
  }
}
