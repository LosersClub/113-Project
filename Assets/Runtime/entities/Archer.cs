using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(CameraBoundsChecker))]
public class Archer : MonoBehaviour {

  [SerializeField]
  private GameObject arrowPrefab;
  [SerializeField]
  private float arrowFiringInterval = 4.0f;

  private SpriteRenderer spriteRenderer;
  private MovementController movementController;
  private CameraBoundsChecker cameraBoundsChecker;

  private bool isFacingRight = true; // will be changed to face player on first Update()
  private Vector2 velocity = Vector2.zero;

  void Start () {
    Assert.IsNotNull(arrowPrefab);

    this.spriteRenderer = this.GetComponent<SpriteRenderer>();
    this.movementController = this.GetComponent<MovementController>();
    this.cameraBoundsChecker = this.GetComponent<CameraBoundsChecker>();

    StartCoroutine(FireArrowsCoroutine());
  }
  
  void Update () {
    this.FacePlayer();

    if(this.isFacingRight && this.spriteRenderer.flipX) {
      this.spriteRenderer.flipX = false;
    }
    else if(!this.isFacingRight && !this.spriteRenderer.flipX) {
      this.spriteRenderer.flipX = true;
    }
  }

  void FixedUpdate() {
    this.velocity += Physics2D.gravity * Time.deltaTime;
    this.movementController.Move(this.velocity * Time.deltaTime);
  }

  private IEnumerator FireArrowsCoroutine() {
    while(true) {
      yield return new WaitForSeconds(this.arrowFiringInterval);
      if(!this.cameraBoundsChecker.IsOutOfBounds()) {
        this.FireArrow();
      }
    }
  }

  private void FireArrow() {
    GameObject arrow = Instantiate(arrowPrefab, this.transform.position, Quaternion.identity);
    Vector2 fireDirection = GameManager.Player.transform.position - this.transform.position;
    arrow.GetComponent<ArcherArrow>().Fire(fireDirection);
  }

  private void FacePlayer() {
    this.isFacingRight = (GameManager.Player.transform.position.x > this.transform.position.x);
  }
}
