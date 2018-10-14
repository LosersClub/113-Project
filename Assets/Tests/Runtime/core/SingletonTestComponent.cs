using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonTestComponent : SingletonMonoBehavior<SingletonTestComponent> {

  // protected constructor prevents external construction of objects,
  // enforcing use of singleton instance:
  protected SingletonTestComponent() {}

}
