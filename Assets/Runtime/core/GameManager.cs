using System;
using UnityEngine;

public class GameManager : SingletonMonoBehavior<GameManager> {
  private GameState state = GameState.ACTIVE;

  [SerializeField]
  private Player player;

  // protected constructor to enforce use of singleton instance:
  protected GameManager() {}

  protected override void Awake() {
    base.Awake();

    if(this == GameManager.Instance) {
      Bindings.Initialize();
    }
  }

  private void Update() {
    InputManager.Update();
  }

  #region Properties
  public static GameManager Manager {
    get {
      return GameManager.Instance;
    }
  }

  public static GameState State {
    get {
      return GameManager.Instance.state;
    }
    private set {
      GameManager.Instance.state = value;
    }
  }

  public static Player Player {
    get {
      return GameManager.Instance.player;
    }
  }
  #endregion
}