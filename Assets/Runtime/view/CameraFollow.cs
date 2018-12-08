using UnityEngine;

public class CameraFollow : MonoBehaviour {

  public float damping = 4f;
  [Range(0.5f, 1f)]
  public float moveDist = 0.75f;
  [Range(0f, 1f)]
  public float offset = 0.5f;

  private Transform player;
  private LevelManager levelManager;
  private Vector2 cameraSize;

  private void Start() {
    this.cameraSize = new Vector2(Camera.main.aspect * Camera.main.orthographicSize * 2,
        Camera.main.orthographicSize * 2);
    player = GameManager.Player.transform;
    this.levelManager = GameManager.LevelManager;
  }

  private void Update() {
    this.FollowPlayer();
  }

  private void FollowPlayer() {
    float leftDist = 1f - moveDist;
    Vector3 newPosition = new Vector3(
      Mathf.Clamp(this.player.position.x, this.cameraSize.x * leftDist - this.offset,
          this.levelManager.Active.Width - this.cameraSize.x * leftDist - this.offset),
      Mathf.Clamp(this.player.position.y, this.cameraSize.y * leftDist - this.offset,
          this.levelManager.Active.Height - this.cameraSize.y * leftDist - this.offset),
      this.transform.position.z);

    Vector3 point = Camera.main.WorldToViewportPoint(newPosition);
    Vector3 left = Camera.main.ViewportToWorldPoint(new Vector3(leftDist, leftDist, point.z));
    Vector3 right = Camera.main.ViewportToWorldPoint(new Vector3(moveDist, moveDist, point.z));
    this.transform.position += new Vector3(
        point.x <= leftDist ? newPosition.x - left.x : point.x >= moveDist ? newPosition.x - right.x : 0f,
        point.y <= leftDist ? newPosition.y - left.y : point.y >= moveDist ? newPosition.y - right.y : 0f,
        point.z);
  }
}
