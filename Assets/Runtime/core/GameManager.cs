using System;
using UnityEngine;

public class GameManager : SingletonMonoBehavior<GameManager> {
  private GameState state = GameState.ACTIVE;

  private void Awake() {
    this.Initialize();
  }

  private void Initialize() {
    Bindings.Initialize();
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