using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class SingletonTests {
  // The SingletonMonoBehavior class is tied to the Unity lifecycle, so the
  // create, reuse, and destroy tests all have side effects and cannot be run
  // fully independently of one another. So, they have to be put together into
  // this one test method:
  [Test]
  public void Singleton_CreatesAndReusesAndDestroysInstance() {
    GameObject gameObject = new GameObject();
    gameObject.AddComponent<SingletonTestComponent>();
    Assert.That(gameObject.GetComponent<SingletonTestComponent>(),
                Is.EqualTo(SingletonTestComponent.Instance));

    GameObject gameObject2 = new GameObject();
    gameObject2.AddComponent<SingletonTestComponent>();
    Assert.That(gameObject.GetComponent<SingletonTestComponent>(),
                Is.EqualTo(SingletonTestComponent.Instance));

    SingletonTestComponent.Instance.OnDestroy();
    Assert.That(() => SingletonTestComponent.Instance,
                Throws.TypeOf<System.ArgumentException>());
  }
}
