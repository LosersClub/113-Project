using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(CameraBoundsChecker))]
[RequireComponent(typeof(Animator))]
public class Archer : MonoBehaviour {

  [SerializeField]
  private GameObject arrowPrefab;
  [SerializeField]
  private float arrowFiringInterval = 4.0f;
  [SerializeField]
  private Vector2 arrowOffset = new Vector2(0.25f, 0.5f);

  private MovementController movementController;
  private CameraBoundsChecker cameraBoundsChecker;
  private Animator animator;

  private bool isFacingRight = true; // will be changed to face player on first Update()
  private bool inAction = false;

  void Start () {
    Assert.IsNotNull(arrowPrefab);

    this.movementController = this.GetComponent<MovementController>();
    this.cameraBoundsChecker = this.GetComponent<CameraBoundsChecker>();
    this.animator = this.GetComponent<Animator>();
  }
  
  void Update () {
    this.FacePlayer();

    Vector2 newVelocity = this.movementController.Velocity + Physics2D.gravity * Time.deltaTime;
    this.movementController.Move(newVelocity * Time.deltaTime);
  }

  public void OnAnimatorStateEnter(AnimatorStateInfo stateInfo) {
    if(stateInfo.IsName("Idle")) {
      this.inAction = false;
      StartCoroutine(WaitThenFireOnceCoroutine());
    }
    else if(stateInfo.IsName("Fire Tell")) {
      this.inAction = true;
    }
    else if(stateInfo.IsName("Fire")) {
      this.FireArrow();
    }
  }

  private IEnumerator WaitThenFireOnceCoroutine() {
    yield return new WaitForSeconds(this.arrowFiringInterval);
    if(this.cameraBoundsChecker.IsOutOfBounds()) {
      StartCoroutine(WaitThenFireOnceCoroutine());
    }
    else {
      this.animator.SetTrigger("Fire Tell");
      // Arrow prefab is fired later, when animator state changes to Fire.
    }
  }

  private void FireArrow() {
    Vector3 arrowPosition = this.transform.position + new Vector3(this.arrowOffset.x * (this.isFacingRight ? 1 : -1), this.arrowOffset.y, 0);
    GameObject arrow = Instantiate(arrowPrefab, arrowPosition, Quaternion.identity);
    Vector2 fireDirection = GameManager.Player.transform.position - this.transform.position;
    arrow.GetComponent<ArcherArrow>().Fire(fireDirection);
  }

  private void FacePlayer() {
    bool newFacingRight = GameManager.Player.transform.position.x > this.transform.position.x;

    if(newFacingRight != this.isFacingRight) {
      this.transform.localScale = new Vector3(this.transform.localScale.x * -1, this.transform.localScale.y, this.transform.localScale.z);
    }

    this.isFacingRight = newFacingRight;
  }
}
