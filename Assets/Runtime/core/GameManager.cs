using System;
using UnityEngine;

public class GameManager : MonoBehaviour {
  private static GameManager manager = null;
  private GameState state = GameState.ACTIVE;

  [SerializeField]
  private Player player;

  private void Awake() {
    if (manager == null) {
      manager = this;
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
    Destroy(manager.gameObject);
    manager = null;
  }

  #region Poperties
  public static GameManager Manager {
    get {
      if (manager != null) {
        return manager;
      }
      throw new ArgumentException("The singleton GameManager instance does not exist!");
    }
  }

  public static GameState State {
    get {
      return manager.state;
    }
  }

  public static Player Player {
    get {
      return manager.player;
    }
  }
  #endregion
}