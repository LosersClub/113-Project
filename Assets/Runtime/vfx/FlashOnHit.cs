using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DamageTaker))]
[RequireComponent(typeof(SpriteRenderer))]
public class FlashOnHit : MonoBehaviour {
  public Color flashColor = new Color(1, 0, 0);
  [Range(0f, 1f)]
  public float maxFlashAmount = 1f;
  [Range(0f, 1f)]
  public float minFlashAmount = 0f;

  public bool useInvulnerabilityDuration = true;
  [Range(0f, 1f), ConditionalHide("useInvulnerabilityDuration", inverse: true)]
  public float duration = 0.5f;

  private Material material;
  private DamageTaker taker;

  private static readonly int flashColorId = Shader.PropertyToID("_FlashColor");
  private static readonly int flashAmountId = Shader.PropertyToID("_FlashAmount");

  private void Awake() {
    this.material = this.GetComponent<SpriteRenderer>().material;
    this.taker = this.GetComponent<DamageTaker>();
    this.taker.OnTakeDamage.AddListener(this.Flash);
  }

  private void OnEnable() {
    this.material.SetFloat(flashAmountId, 0f);
  }

  private void Flash(DamageDealer dealer, DamageTaker taker) {
    this.material.SetFloat(flashAmountId, this.maxFlashAmount);
    this.material.SetColor(flashColorId, this.flashColor);
    if (this.useInvulnerabilityDuration) {
      this.duration = this.taker.InvulnerabilityDuration;
    }
    this.StartCoroutine(this.UpdateFlash());
  }

  private IEnumerator UpdateFlash() {
    float timer = duration;
    float halfDur = duration/2f;
    float currentAmount = this.maxFlashAmount;
    while (timer > 0) {
      timer -= Time.deltaTime;
      currentAmount = Mathf.Lerp(currentAmount, this.minFlashAmount,
        Time.deltaTime/halfDur);
      this.material.SetFloat(flashAmountId, currentAmount);
      yield return null;
    }
    this.material.SetFloat(flashAmountId, 0f);
  }
}