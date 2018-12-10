using UnityEngine;

public class BountySpike : MonoBehaviour {

  private BoxCollider2D box;

  private void Awake() {
    this.box = this.GetComponent<BoxCollider2D>();
  }

  public void EnableHitbox() {
    this.box.enabled = true;
  }

  public void DisableHitbox() {
    this.box.enabled = false;
  }
}