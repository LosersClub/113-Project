using System;
using UnityEngine;

public class GameManager : SingletonMonoBehavior<GameManager> {
  private GameState state = GameState.ACTIVE;

  // protected constructor prevents external construction of objects,
  // enforcing use of singleton instance:
  protected GameManager() {}

  protected override void Awake() {
    base.Awake();

    // Only do the following once - when Instance is Awake:
    if(this == GameManager.Instance) {
      Bindings.Initialize();
    }
  }

  private void Update() {
    InputManager.Update();
  }

  #region Properties
  public GameState State {
    get {
      return this.state;
    }
    private set {
      this.state = value;
    }
  }
  #endregion
}