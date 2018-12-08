using UnityEngine;

public class LevelManager : MonoBehaviour {
  public Level[] levels;

  public GameObject LeftWall { get; private set; }
  public GameObject RightWall { get; private set; }
  public Level Active { get; private set; }

  private GameObject levelManager;
  private int activeLevel = 0;

  private void Awake() {
    this.levelManager = new GameObject("Level Manager");
    this.levelManager.transform.position = Vector3.zero;

    this.LeftWall = new GameObject("Left Wall");
    this.LeftWall.AddComponent<BoxCollider2D>();
    this.LeftWall.transform.SetParent(this.levelManager.transform);
    this.LeftWall.layer = LayerMask.NameToLayer("Impassable");
    this.LeftWall.SetActive(false);

    this.RightWall = new GameObject("Right Wall");
    this.RightWall.AddComponent<BoxCollider2D>();
    this.RightWall.transform.SetParent(this.levelManager.transform);
    this.RightWall.layer = LayerMask.NameToLayer("Impassable");
    this.RightWall.SetActive(false);
  }

  // TODO: Delete
  private void OnEnable() {
    this.StartFirstLevel();
  }

  public void StartFirstLevel() {
    this.activeLevel = 0;
    this.StartLevel();
  }

  public void NextLevel() {
    this.activeLevel++;
    if (this.activeLevel >= this.levels.Length) {
      // TODO, no more levels idk what to do?
      return;
    }

    Destroy(this.Active.gameObject);
    this.StartLevel();
  }

  public void StartLevel() {
    this.Active = Instantiate<Level>(this.levels[this.activeLevel]);
    this.Active.transform.SetParent(this.levelManager.transform);
    this.Active.transform.localPosition = Vector3.zero;
    this.Active.gameObject.SetActive(true);
    this.Active.Enable();
  }

  public void SetWalls(int width, int maxHeight) {
    this.LeftWall.transform.localScale = new Vector3(1, maxHeight, 1);
    this.RightWall.transform.localScale = new Vector3(1, maxHeight, 1);

    this.LeftWall.transform.localPosition = new Vector3(-1, maxHeight / 2);
    this.RightWall.transform.localPosition = new Vector3(width, maxHeight / 2);
  }
}