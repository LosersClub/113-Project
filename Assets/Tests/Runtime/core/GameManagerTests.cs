using NUnit.Framework;
using UnityEngine;

public class GameManagerTests {
  [Test]
  public void AssignInstance() {
    GameObject gameObject = new GameObject();
    gameObject.AddComponent<GameManager>();
    Assert.That(gameObject.GetComponent<GameManager>(), Is.EqualTo(GameManager.Manager));

    GameObject gameObject2 = new GameObject();
    gameObject2.AddComponent<GameManager>();
    Assert.That(gameObject.GetComponent<GameManager>(), Is.EqualTo(GameManager.Manager));

    GameManager.Destroy();
    Assert.That(() => GameManager.Manager, Throws.TypeOf<System.ArgumentException>());
  }

  [Test]
  public void GetNoInstance() {
    Assert.That(() => GameManager.Manager, Throws.TypeOf<System.ArgumentException>());
  }
}