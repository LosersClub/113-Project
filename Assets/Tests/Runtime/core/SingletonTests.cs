using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class SingletonTests {
  // The SingletonMonoBehavior class is tied to the Unity lifecycle, so all the
  // tests have side effects and cannot be run fully independently of one
  // another. So, they have to be put together into this one test method:
  [Test]
  public void Singleton_CreatesAndDestroysDupesAndDestroysInstance() {
    // Test that can create a new component on first Instance call:
    SingletonTestComponent firstInstance = SingletonTestComponent.Instance;
    Assert.That(firstInstance, Is.Not.Null);

    GameObject toDestroyObj = new GameObject();
    toDestroyObj.AddComponent<SingletonTestComponent>();
    // Adding the component creates a second component, which should get
    // destroyed by SingletonMonoBehavior. But it won't get destroyed
    // immediately, so before it is destroyed, make sure that the singleton
    // Instance wasn't changed to the new component:
    Assert.That(toDestroyObj.GetComponent<SingletonTestComponent>(),
                  Is.Not.EqualTo(SingletonTestComponent.Instance));
    Assert.That(firstInstance, Is.EqualTo(SingletonTestComponent.Instance));
    // Make sure that the component was not already destroyed. If it was, then
    // the tests above are inaccurate:
    Assert.That(toDestroyObj.GetComponent<SingletonTestComponent>(),
                Is.Not.Null);

    // TODO: wait until toDestroyObj's SingletonTestComponent is destroyed.
    // Have to wait because SingletonTestComponent uses
    // UnityEngine.Object.Destroy, which does not immediately destroy the
    // component.
    // Until that TODO is complete, destroy the object immediately to proceed
    // with testing:
    var toDestroyComp = toDestroyObj.GetComponent<SingletonTestComponent>();
    UnityEngine.Object.DestroyImmediate(toDestroyComp);
    // Note: have to test via toDestroyObj.GetComponent. Directly testing
    // toDestroyComp Is.Null fails, stating "Expected null but was <null>" for
    // some reason.
    Assert.That(toDestroyObj.GetComponent<SingletonTestComponent>(),
                Is.Null);

    GameObject firstInstanceObj = firstInstance.gameObject;
    UnityEngine.Object.DestroyImmediate(firstInstance);
    // Note: have to test via firstInstanceObj.GetComponent. Directly testing
    // firstInstance Is.Null fails, stating "Expected null but was <null>" for
    // some reason.
    Assert.That(firstInstanceObj.GetComponent<SingletonTestComponent>(),
                Is.Null);

    Assert.That(() => { return SingletonTestComponent.Instance; },
                Throws.ArgumentException);
  }
}
