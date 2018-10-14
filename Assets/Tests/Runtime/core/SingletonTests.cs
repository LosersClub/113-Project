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

    GameObject toDestroyObject = new GameObject();
    toDestroyObject.AddComponent<SingletonTestComponent>();
    // Adding the component creates a second component, which should get
    // destroyed by SingletonMonoBehavior. But it won't get destroyed
    // immediately, so before it is destroyed, make sure that the singleton
    // Instance wasn't changed to the new component:
    Assert.That(toDestroyObject.GetComponent<SingletonTestComponent>(),
                  Is.Not.EqualTo(SingletonTestComponent.Instance));
    // Make sure the Instance is still the first instance:
    Assert.That(firstInstance, Is.EqualTo(SingletonTestComponent.Instance));
    // Finally, make sure that the component was not already destroyed. If it
    // was, then the tests above are inaccurate:
    Assert.That(toDestroyObject.GetComponent<SingletonTestComponent>(),
                Is.Not.Null);

    // TODO: wait until toDestroyObject's SingletonTestComponent is destroyed.
    // Have to wait because SingletonTestComponent calls
    // UnityEngine.Object.Destroy to destroy the component, but that method
    // does not immediately destroy the component. For now, instead of waiting,
    // destroy the object immediately here, and then proceed with testing:
    UnityEngine.Object.DestroyImmediate(toDestroyObject.GetComponent<SingletonTestComponent>());
    // Make sure that the component was destroyed:
    Assert.That(toDestroyObject.GetComponent<SingletonTestComponent>(),
                Is.Null);

    var firstInstanceObj = firstInstance.gameObject;
    UnityEngine.Object.DestroyImmediate(firstInstance);
    // Have to test that firstInstanceObj.GetComponent<SingletonTestComponent>()
    // Is.Null. Testing firstInstance Is.Null fails, stating "Expected null but
    // was <null>" for some reason.
    Assert.That(firstInstanceObj.GetComponent<SingletonTestComponent>(), Is.Null);

    Assert.That(() => { return SingletonTestComponent.Instance; }, Throws.ArgumentException);
  }
}
