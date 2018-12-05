﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(EnemyComponent))]
[RequireComponent(typeof(ArcherStationaryController))]
public class Archer : MonoBehaviour {

  [SerializeField]
  private GameObject arrowPrefab;
  [SerializeField]
  private float arrowFiringInterval = 4.0f;
  [SerializeField]
  private Vector2 arrowOffset = new Vector2(0.25f, 0.5f);

  private EnemyComponent enemyComponent;
  private ArcherStationaryController archerStationaryController;

  void Start () {
    Assert.IsNotNull(arrowPrefab);

    this.enemyComponent = this.GetComponent<EnemyComponent>();
    this.archerStationaryController = this.GetComponent<ArcherStationaryController>();
  }

  public void OnAnimatorStateEnter(AnimatorStateInfo stateInfo) {
    if(stateInfo.IsName("Idle")) {
      this.enemyComponent.inAction = false;
      StartCoroutine(WaitThenFireOnceCoroutine());
    }
    else if(stateInfo.IsName("Fire Tell")) {
      this.enemyComponent.inAction = true;
    }
    else if(stateInfo.IsName("Fire")) {
      this.FireArrow();
    }
  }

  private IEnumerator WaitThenFireOnceCoroutine() {
    yield return new WaitForSeconds(this.arrowFiringInterval);
    if(this.archerStationaryController.InFiringRange()) {
      this.enemyComponent.Anim.SetTrigger("Fire Tell");
      // Arrow prefab is fired later, when animator state changes to Fire.
    }
    else {
      StartCoroutine(WaitThenFireOnceCoroutine());
    }
  }

  private void FireArrow() {
    Vector3 arrowPosition = this.transform.position + new Vector3(this.arrowOffset.x * (this.enemyComponent.FacingRight ? 1 : -1), this.arrowOffset.y, 0);
    GameObject arrow = Instantiate(arrowPrefab, arrowPosition, Quaternion.identity);
    Vector2 fireDirection = GameManager.Player.transform.position - this.transform.position;
    arrow.GetComponent<ArcherArrow>().Fire(fireDirection);
  }
}
