using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerBulletSpawner : MonoBehaviour {
  [SerializeField]
  private float distance = 2f;
  [SerializeField]
  private GameObject bulletPrefab;

  private new SpriteRenderer renderer;
  private Vector3 direction;
  private float angle;

  private void Awake() {
    this.renderer = this.GetComponent<SpriteRenderer>();
  }

  public void UpdatePosition(float x, float y) {
    if (x == 0 && y == 0) {
      this.direction = new Vector3(1f, 0f);
      this.transform.rotation = Quaternion.identity;
      this.transform.localPosition = new Vector3(distance, 0f, 0f);
      this.renderer.flipY = false;
      return;
    }
    this.direction = new Vector3(x, y);
    direction.Normalize();
    this.transform.localPosition = this.direction * this.distance;
    this.angle = Mathf.Atan2(this.direction.y, this.direction.x) * Mathf.Rad2Deg;
    this.transform.rotation = Quaternion.AngleAxis(this.angle, Vector3.forward);
    this.renderer.flipY = Mathf.Abs(this.angle) > 90f;
  }

  public void SpawnBullet() {
    GameObject clone = Instantiate(bulletPrefab);
    clone.transform.position = this.transform.position;
    clone.GetComponent<Bullet>().direction = this.direction;
  }
}