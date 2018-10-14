using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonTestComponent : SingletonMonoBehavior<SingletonTestComponent> {

  // protected constructor prevents external construction of objects,
  // enforcing use of singleton instance:
  protected SingletonTestComponent() {}

  // Override Awake to test that this does not break SingletonMonoBehavior:
  protected override void Awake() {
    base.Awake();
    Debug.Log("SingletonTestComponent Awake");
  }

  // Override OnDestroy to test that this does not break SingletonMonoBehavior:
  protected override void OnDestroy() {
    base.OnDestroy();
    Debug.Log("SingletonTestComponent OnDestroy");
  }
}
