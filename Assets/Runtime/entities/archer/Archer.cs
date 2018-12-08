using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(EnemyComponent))]
public class Archer : MonoBehaviour {

  [SerializeField]
  private GameObject arrowPrefab;
  [SerializeField]
  private Component archerFireCheckerComponent; // a component that implements IArcherFireChecker
  [SerializeField]
  private float arrowFiringInterval = 4.0f;
  [SerializeField]
  private Vector2 arrowOffset = new Vector2(0.25f, 0.5f);

  private EnemyComponent enemyComponent;
  private IArcherFireChecker archerFireChecker;

  void Start () {
    Assert.IsNotNull(this.arrowPrefab);
    Assert.IsNotNull(this.archerFireCheckerComponent);

    this.enemyComponent = this.GetComponent<EnemyComponent>();
    this.archerFireChecker = this.archerFireCheckerComponent as IArcherFireChecker;
    // Will be null if archerFireCheckerComponent does not implement IArcherFireChecker:
    Assert.IsNotNull(this.archerFireChecker);
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
    bool fired = false;
    while(!fired) {
      yield return new WaitForSeconds(this.arrowFiringInterval);
      if(this.archerFireChecker.CanFire()) {
        this.enemyComponent.Anim.SetTrigger("Fire Tell");
        // Arrow prefab is fired later, when animator state changes to Fire.
        fired = true;
      }
    }
  }

  private void FireArrow() {
    Vector3 arrowPosition = this.transform.position + new Vector3(this.arrowOffset.x * (this.enemyComponent.FacingRight ? 1 : -1), this.arrowOffset.y, 0);
    GameObject arrow = Instantiate(arrowPrefab, arrowPosition, Quaternion.identity);
    // Add arrowOffset.y to direction, to fire high enough that player can crouch underneath:
    Vector2 fireDirection = GameManager.Player.transform.position - arrowPosition + new Vector3(0, this.arrowOffset.y, 0);
    arrow.GetComponent<ArcherArrow>().Fire(fireDirection);
  }
}
