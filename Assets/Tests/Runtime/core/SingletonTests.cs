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
    GameObject firstComponentObj = new GameObject();
    firstComponentObj.AddComponent<SingletonTestComponent>();
    Assert.That(firstComponentObj.GetComponent<SingletonTestComponent>(),
                  Is.EqualTo(SingletonTestComponent.Instance));

    GameObject toDestroyComponentObj = new GameObject();
    // Adding the component creates a second component, which should get
    // destroyed by SingletonMonoBehavior. But it won't get destroyed
    // immediately, so before it is destroyed, make sure that the singleton
    // Instance wasn't changed to the new component:
    toDestroyComponentObj.AddComponent<SingletonTestComponent>();
    // Make sure that Instance wasn't changed to the second component:
    Assert.That(toDestroyComponentObj.GetComponent<SingletonTestComponent>(),
                  Is.Not.EqualTo(SingletonTestComponent.Instance));
    // Make sure the Instance is still the first component:
    Assert.That(firstComponentObj.GetComponent<SingletonTestComponent>(),
                  Is.EqualTo(SingletonTestComponent.Instance));
    // Finally, make sure that the component was not already destroyed. If it
    // was, then the tests above are inaccurate:
    Assert.That(toDestroyComponentObj.GetComponent<SingletonTestComponent>(),
                Is.Not.Null);

    // TODO: wait until toDestroyComponentObj is destroyed. Then, destroy
    // firstComponentObj, and wait for it to be destroyed. Then, test that
    // Instance was also destroyed, and that attempting to access Instance
    // raises Exception. Note that calling UnityEngine.Object.Destroy(obj) will
    // not immediately destroy obj; this complicates testing.
  }
}
