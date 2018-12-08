using System;
using UnityEngine;

[RequireComponent(typeof(CameraShake))]
public class GameManager : SingletonMonoBehavior<GameManager> {
  private GameState state = GameState.ACTIVE;

  [SerializeField]
  private Player player;
  private CameraShake cameraShake;
  private LoadingScreen loadingScreen;
  private LevelManager levelManager;

  // protected constructor to enforce use of singleton instance:
  protected GameManager() {}

  protected override void Awake() {
    base.Awake();

    if(this == GameManager.Instance) {
      Bindings.Initialize();
    }
    this.cameraShake = this.GetComponent<CameraShake>();
    this.loadingScreen = Camera.main.GetComponent<LoadingScreen>();
    this.levelManager = this.GetComponent<LevelManager>();

    Debug.Log(2 * Camera.main.orthographicSize + " " + Camera.main.aspect * Camera.main.orthographicSize * 2);
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

  public static CameraShake CameraShake {
    get {
      return GameManager.Instance.cameraShake;
    }
  }

  public static LoadingScreen LoadingScreen {
    get {
      return GameManager.Instance.loadingScreen;
    }
  }

  public static LevelManager LevelManager {
    get {
      return GameManager.Instance.levelManager;
    }
  }
  #endregion
}