using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ManualRoom))]
public class BountyRoom : MonoBehaviour {
  public GameObject bounty;

  private ManualRoom room;
  private bool done = false;

  private void Awake() {
    this.room = this.GetComponent<ManualRoom>();
    this.room.events.generate.AddListener(this.Generate);
    this.room.events.generate.AddListener(this.StartRoom);
  }

  private void Generate(Room room) {

  }

  private void StartRoom(Room room) {
    this.StartCoroutine(this.RightWall());
  }

  private IEnumerator RightWall() {
    yield return new WaitForSeconds(2f);
    GameManager.LevelManager.RightWall.SetActive(true);
  }

  public void OnBountyDie(DamageDealer dealer, DamageTaker taker) {

  }
}