using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ManualRoom))]
public class BountyRoom : MonoBehaviour {
  public GameObject bounty;

  private ManualRoom room;
  private bool done = false;
  private GameObject bountyInstance;
  private GameObject RightWall;

  private void Awake() {
    this.room = this.GetComponent<ManualRoom>();
    this.room.events.generate.AddListener(this.Generate);
  }

  private void Generate(Room room) {
    this.bountyInstance = Instantiate(bounty);
    this.bountyInstance.transform.SetParent(GameManager.LevelManager.Active.transform);
    this.bountyInstance.transform.position = new Vector2(room.Width - 5, 2f);
    this.bountyInstance.SetActive(true);

    this.RightWall = new GameObject("Right Wall");
    this.RightWall.AddComponent<BoxCollider2D>();
    this.RightWall.transform.SetParent(GameManager.LevelManager.Active.transform);
    this.RightWall.layer = LayerMask.NameToLayer("Impassable");
    this.RightWall.SetActive(true);
    this.RightWall.transform.localScale = new Vector3(1, room.Height, 1);
    this.RightWall.transform.localPosition = new Vector3(room.Width, room.Height / 2);
  }
}