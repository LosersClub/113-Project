using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(SpriteRenderer))]
public class Archer : MonoBehaviour {

  [SerializeField]
  private GameObject arrowPrefab;
  [SerializeField]
  private float arrowFiringInterval = 4.0f;
  [SerializeField]
  private bool isInitiallyFacingRight = true;

  private SpriteRenderer spriteRenderer;

  private bool isFacingRight;

  void Start () {
    Assert.IsNotNull(arrowPrefab);

    this.spriteRenderer = this.GetComponent<SpriteRenderer>();

    this.isFacingRight = this.isInitiallyFacingRight;

    StartCoroutine(FireArrowsCoroutine());
  }
  
  void Update () {
    if(this.isFacingRight && this.spriteRenderer.flipX) {
      this.spriteRenderer.flipX = false;
    }
    else if(!this.isFacingRight && !this.spriteRenderer.flipX) {
      this.spriteRenderer.flipX = true;
    }
  }

  private IEnumerator FireArrowsCoroutine() {
    while(true) {
      yield return new WaitForSeconds(this.arrowFiringInterval);
      this.FireArrow();
    }
  }

  private void FireArrow() {
    GameObject arrow = Instantiate(arrowPrefab, this.transform.position, Quaternion.identity);
    arrow.GetComponent<ArcherArrow>().Fire(new Vector2(this.isFacingRight ? 1 : -1, 0));
  }
}
