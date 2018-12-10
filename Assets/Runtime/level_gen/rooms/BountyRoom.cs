using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ManualRoom))]
public class BountyRoom : MonoBehaviour {
  public GameObject bounty;

  private ManualRoom room;
  private bool done = false;
  private GameObject bountyInstance;

  private void Awake() {
    this.room = this.GetComponent<ManualRoom>();
    this.room.events.generate.AddListener(this.Generate);
  }

  private void Generate(Room room) {
    this.bountyInstance = Instantiate(bounty);
    this.bountyInstance.transform.position = new Vector2(room.Width - 5, 2f);
    this.bountyInstance.SetActive(true);
  }

  private void Update() {
    GameManager.LevelManager.RightWall.SetActive(true);
  }

  public void OnBountyDie(DamageDealer dealer, DamageTaker taker) {

  }
}