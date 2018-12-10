using UnityEngine;
using System.Collections;

public class Dissolve : MonoBehaviour {
  private static readonly int dissolveId = Shader.PropertyToID("_Level");

  private Material material;

  private void Awake() {
    this.material = this.GetComponent<SpriteRenderer>().material;
  }

  public IEnumerator DoDissolve(float duration) {
    float timer = 0;
    float dissolve = 0f;
    this.material.SetFloat(dissolveId, 0f);
    while (dissolve < 0.98f) {
      dissolve = Mathf.Lerp(dissolve, 1f, timer += Time.deltaTime / duration);
      this.material.SetFloat(dissolveId, dissolve);
      yield return null;
    }
    this.material.SetFloat(dissolveId, 1f);
  }
}