using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Archer : MonoBehaviour {

  [SerializeField]
  private GameObject arrowPrefab;
  [SerializeField]
  private float arrowFiringInterval = 4.0f;

  void Start () {
    Assert.IsNotNull(arrowPrefab);

    StartCoroutine(FireArrowsCoroutine());
  }
  
  void Update () {

  }

  private IEnumerator FireArrowsCoroutine() {
    while(true) {
      yield return new WaitForSeconds(this.arrowFiringInterval);
      this.FireArrow();
    }
  }

  private void FireArrow() {
    GameObject arrow = Instantiate(arrowPrefab, this.transform.position, Quaternion.identity);
    arrow.GetComponent<ArcherArrow>().Fire();
  }
}
