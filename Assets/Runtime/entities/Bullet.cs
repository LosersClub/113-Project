using UnityEngine;

[RequireComponent(typeof(ColliderDamageDealer))]
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(CircleCollider2D))]
public class Bullet : MonoBehaviour {
  public float speed = 1f;
  public float lifeTime = 1f;
  [HideInInspector]
  public Vector2 direction;

  private MovementController movement;
  private float timer;

  private void OnEnable() {
    this.movement = this.GetComponent<MovementController>();
    this.timer = 0f;
  }

  public void Update() {
    timer += Time.deltaTime;
    this.movement.Move(direction * speed * Time.deltaTime); // TODO: Replace with its own thing or support single ray
    if (timer > lifeTime || this.movement.Collision.Collision) {
      Destroy(this.gameObject); // TODO: disable and return to pool
    }
  }
}