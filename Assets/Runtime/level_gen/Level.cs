using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public abstract class Level : MonoBehaviour  {
  [Header("Generation Settings")]
  public bool randomSeed = true;
  [ConditionalHide("randomSeed", inverse: true)]
  public int seed = 42;
  public EnemySpawner enemySpawner;

  [Header("Transition Settings")]
  [Range(0f, 1f)]
  public float pauseDuration = 0.5f;
  public float playerPause = 0.25f;
  public float playerWalkDistance = 4f;
  public GameObject background;
  public AudioClip track;

  #region Properties
  public Player Player { get; private set; }
  public LoadingScreen LoadingScreen { get; private set; }
  public LevelManager LevelManager { get; private set; }
  public Room ActiveRoom {
    get {
      return this.rooms[this.active];
    }
  }
  public int Width { get { return this.ActiveRoom.Width; } }
  public int Height { get { return this.ActiveRoom.Height; } }
  #endregion

  private Room[] rooms;
  [SerializeField, ReadOnly]
  private int active;

  private void Awake() {
    this.Player = GameManager.Player;
    this.LoadingScreen = GameManager.LoadingScreen;
    this.LevelManager = GameManager.LevelManager;
  }

  public void Enable() {
    if (!randomSeed) {
      Random.InitState(this.seed);
    }

    this.LoadingScreen.Set(1f);

    this.enemySpawner.Start(this);

    this.rooms = this.GenerateRooms();
    this.active = 0;
    this.LoadActiveRoom();
  }

  private void LoadActiveRoom() {
    this.RenderRoom(this.ActiveRoom);
    this.LevelManager.SetWalls(this.ActiveRoom.Width, this.ActiveRoom.Height + 20);
    this.LevelManager.LeftWall.SetActive(false);
    this.LevelManager.RightWall.SetActive(true);
    this.StartCoroutine(this.EnterRoom());
  }

  private IEnumerator EnterRoom() {
    yield return this.StartCoroutine(this.LoadingScreen.FadeOut());
    this.Player.transform.position = new Vector3(this.transform.position.x - 2, this.transform.position.y + 2.5f);
    this.Player.gameObject.SetActive(true);

    if (!(this.ActiveRoom is MonoRoom) || ((MonoRoom)this.ActiveRoom).UseBlockers) {
      this.LevelManager.StartBlockers(this.ActiveRoom.Width, this.ActiveRoom.Height);
    }

    yield return this.StartCoroutine(this.Player.EnterRoom(this.playerWalkDistance, this.playerPause));
    Camera.main.GetComponent<CameraFollow>().enabled = true;
    this.LevelManager.LeftWall.SetActive(true);
    if (!(this.ActiveRoom is MonoRoom) || ((MonoRoom)this.ActiveRoom).UseSpawner) {
      this.enemySpawner.StartRoom(this.ActiveRoom);
    }
    if (this.ActiveRoom is MonoRoom) {
      ((MonoRoom)this.ActiveRoom).StartRoom();
      if (!((MonoRoom)this.ActiveRoom).UseBlockers) {
        this.StartCoroutine(this.ExitRoom());
      }
    }

    if (!(this.ActiveRoom is MonoRoom) || ((MonoRoom)this.ActiveRoom).UseBlockers) {
      this.LevelManager.StartDamagers(this.ActiveRoom.Width, this.ActiveRoom.Height);
    }
  }

  public IEnumerator ExitRoom() {
    this.LevelManager.StopBlockers();
    this.LevelManager.RightWall.SetActive(false);
    while (this.Player.transform.position.x < this.transform.position.x + this.ActiveRoom.Width) {
      yield return null;
    }
    this.Player.gameObject.SetActive(false);
    yield return this.StartCoroutine(this.LoadingScreen.FadeIn());
    Camera.main.transform.position = new Vector3(13.5f, 7.5f, -10f);
    Camera.main.GetComponent<CameraFollow>().enabled = false;
    yield return new WaitForSeconds(this.pauseDuration);
    this.UnloadRoom(this.ActiveRoom);
    this.active++;
    if (this.active < this.rooms.Length) {
      this.LoadActiveRoom();
    } else {
      this.LevelManager.NextLevel();
    }
  }

  public abstract Room[] GenerateRooms();
  public abstract void RenderRoom(Room room);
  public abstract void UnloadRoom(Room room);
}