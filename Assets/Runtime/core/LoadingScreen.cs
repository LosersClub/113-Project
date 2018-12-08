using UnityEngine;
using System.Collections;

public class LoadingScreen : MonoBehaviour {
  public const float Precision = 0.02f;

  [SerializeField]
  private Material transition;
  [SerializeField, Range(0f, 1f)]
  private float cutoffDuration = 1f;
  
  private readonly int cutoffId = Shader.PropertyToID("_Cutoff");
  private float cutoff = 0f;

  public float Cutoff {
    get {
      return this.cutoff;
    }
    set {
      this.transition.SetFloat(this.cutoffId, this.cutoff = value);
    }
  }

  private void Awake() {
    this.Cutoff = 0;
  }

  private void OnRenderImage(RenderTexture source, RenderTexture destination) {
    Graphics.Blit(source, destination, transition);
  }

  public void Set(float value) {
    this.Cutoff = value;
  }

  public IEnumerator FadeIn(float duration, float goal) {
    float timer = 0;
    while (cutoff < goal - Precision) {
      this.Cutoff = Mathf.Lerp(this.cutoff, goal, timer += Time.deltaTime / duration);
      yield return null;
    }
    this.Cutoff = goal;
  }

  public IEnumerator FadeOut(float duration, float goal) {
    float timer = 0;
    while (cutoff > goal + Precision) {
      this.Cutoff = Mathf.Lerp(this.cutoff, goal, timer += Time.deltaTime / duration);
      yield return null;
    }
    this.Cutoff = goal;
  }

  public IEnumerator FadeIn() {
    return FadeIn(this.cutoffDuration, 1f);
  }
  public IEnumerator FadeOut() {
    return FadeOut(this.cutoffDuration, 0f);
  }
}