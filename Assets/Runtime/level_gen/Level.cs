using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public abstract class Level : MonoBehaviour  {

  //[MinMax(28, 500)]
  //public MinMax roomWdith = new MinMax(28, 100);
  //[MinMax(16, 200)]
  //public MinMax roomHeight = new MinMax(16, 50);

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
    this.Player.transform.position = new Vector3(this.transform.position.x - 2, this.transform.position.y + 2.5f);
    this.Player.gameObject.SetActive(true);
    yield return this.StartCoroutine(this.LoadingScreen.FadeOut());
    yield return this.StartCoroutine(this.Player.EnterRoom(this.playerWalkDistance, this.playerPause));
    this.LevelManager.LeftWall.SetActive(true);

    this.enemySpawner.StartRoom(this.ActiveRoom);
    // TODO: Level Blockers Coroutine (start player enter at same time?)
  }

  public IEnumerator ExitRoom() {
    // TODO: Make blockers put lasers down and leave
    this.LevelManager.RightWall.SetActive(false);
    while (this.Player.transform.position.x < this.transform.position.x + this.ActiveRoom.Width) {
      yield return null;
    }
    yield return this.StartCoroutine(this.LoadingScreen.FadeIn());
    yield return new WaitForSeconds(this.pauseDuration);  
    this.Player.gameObject.SetActive(false);
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