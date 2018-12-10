using System;
using UnityEngine;

[RequireComponent(typeof(CameraShake))]
public class GameManager : SingletonMonoBehavior<GameManager> {
  private GameState state = GameState.ACTIVE;

  [SerializeField]
  private Player player;
  private CameraShake cameraShake;
  private LevelManager levelManager;
  private LoadingScreen loadingScreen;
  private AudioManager audioManager;

  // protected constructor to enforce use of singleton instance:
  protected GameManager() {}

  protected override void Awake() {
    base.Awake();

    if(this == GameManager.Instance) {
      Bindings.Initialize();
    }
    this.cameraShake = this.GetComponent<CameraShake>();
    this.levelManager = this.GetComponent<LevelManager>();
    this.loadingScreen = this.GetComponent<LoadingScreen>();
    this.audioManager = this.GetComponent<AudioManager>();
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

  public static LevelManager LevelManager {
    get {
      return GameManager.Instance.levelManager;
    }
  }

  public static AudioManager AudioManager {
    get {
      return GameManager.Instance.audioManager;
    }
  }

  public static LoadingScreen LoadingScreen {
    get {
      return GameManager.Instance.loadingScreen;
    }
  }
  #endregion
}