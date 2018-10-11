using System;
using UnityEngine;

public class GameManager : MonoBehaviour {
  private static GameManager manager = null;
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

  private GameState state = GameState.ACTIVE;
  public GameState State {
    get {
      return this.state;
    }
    private set {
      this.state = value;
    }
  }

  private void Awake() {
    if (manager == null) {
      GameManager.Manager = this;
    } else if (manager != this) {
      Destroy(this.gameObject); // Delete current instance to maintain singleton.
    }
    Bindings.Initialize();
    DontDestroyOnLoad(manager);
  }

  protected static void Destroy() {
    Destroy(GameManager.Manager.gameObject);
    GameManager.Manager = null;
  }
}