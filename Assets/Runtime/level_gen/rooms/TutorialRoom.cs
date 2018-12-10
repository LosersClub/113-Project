using UnityEngine;
public class TutorialRoom : MonoBehaviour {

  public GameObject alter;
  public GameObject templeWall;
  public GameObject tutorial;

  private ManualRoom room;
  private GameObject wall;
  private GameObject altar;
  private GameObject tutorialInstance;

  private void Awake() {
    this.room = this.GetComponent<ManualRoom>();
    this.room.events.generate.AddListener(this.Generate);
    this.room.events.startRoom.AddListener(this.StartRoom);
    this.room.events.unload.AddListener(this.Unload);
  }

  private void Generate(Room room) {
    this.wall = Instantiate(this.templeWall);
    this.wall.transform.position = new Vector2(-1, 8f);
    this.wall.SetActive(true);

    this.altar = Instantiate(this.alter);
    this.altar.transform.position = new Vector2(1f, 2f);
    this.altar.SetActive(true);

    this.tutorialInstance = Instantiate(this.tutorial);
    this.tutorialInstance.transform.position = Vector3.zero;
    this.tutorialInstance.SetActive(true);
  }

  private void StartRoom(Room room) {
    
  }

  private void Unload(Room room) {
    Destroy(this.wall);
    Destroy(this.altar);
    Destroy(this.tutorialInstance);
  }
}