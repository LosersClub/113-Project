using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
  public Image[] hearts;
  public Image[] ammo;
  public Sprite heartSprite;
  public Sprite emptyAmmoSprite;
  public Sprite fullAmmoSprite;

  private int numAmmo;
  private int numHearts;

  private void Awake() {
    this.GetComponent<DamageTaker>().OnTakeDamage.AddListener(this.OnDamage);
    this.numAmmo = ammo.Length;
    this.numHearts = hearts.Length;
  }

  public void OnDamage(DamageDealer dealer, DamageTaker taker) {
    float newHealth = this.numHearts - dealer.Damage;
    while (this.numHearts > 0 && this.numHearts > newHealth) {
      this.hearts[--this.numHearts].enabled = false;
    }
  }

  public void ReduceAmmo() {
    if (this.numAmmo == 0) {
      return;
    }
    this.ammo[--this.numAmmo].sprite = emptyAmmoSprite;
  }

  public void GainAmmo() {
    if (this.numAmmo == this.ammo.Length) {
      return;
    }
    this.ammo[this.numAmmo++].sprite = fullAmmoSprite;
  }
}