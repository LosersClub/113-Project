using System;
using UnityEngine;

public class GameManager : MonoBehaviour {
  private static GameManager manager = null;
  private GameState state = GameState.ACTIVE;

  private void Awake() {
    if (manager == null) {
      GameManager.Manager = this;
      DontDestroyOnLoad(manager);
      this.Initialize();
    } else if (manager != this) {
      Destroy(this.gameObject); // Delete current instance to maintain singleton.
    }
  }

  private void Initialize() {
    Bindings.Initialize();
  }

  private void Update() {
    InputManager.Update();
  }

  public static void Destroy() {
    Destroy(GameManager.Manager.gameObject);
    GameManager.Manager = null;
  }

  #region Poperties
  public static GameManager Manager {
    get {
      if (manager != null) {
        return manager;
      }
      throw new ArgumentException("The singleton GameManager instance does not exist!");
    }
    private set {
      manager = value;
    }
  }

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