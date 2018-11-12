using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
  private new Transform camera;

  public void Awake() {
    this.camera = Camera.main.transform;
  }

  public void ShakeCamera(float duration, float distance) {
    this.StartCoroutine(this.Shake(duration, distance));
  }

  private IEnumerator Shake(float duration, float distance) {
    Vector3 origin = this.camera.localPosition;
    for (float timer = duration; timer > 0;) {
      this.camera.localPosition = origin + Random.insideUnitSphere * distance;
      timer -= Time.deltaTime;
      yield return null;
    }
    this.camera.localPosition = origin;
  }
}