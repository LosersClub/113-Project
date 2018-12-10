using UnityEngine;

public class DamageTaker : MonoBehaviour {
  [SerializeField, ReadOnly]
  protected float currentHealth;
  [SerializeField]
  protected float maxHealth = 5.0f;
  [SerializeField]
  protected float invulnerabilityDuration = 1f;
  [SerializeField]
  protected bool disableOnDeath = false;
  [SerializeField]
  protected bool resetOnEnable = true;

  public HealEvent OnGainHealth;
  public DamageEvent OnTakeDamage;
  public DamageEvent OnDie;

  protected Vector2 damageDirection;
  protected bool invulnerable;
  protected float invulnerableTimer;

  public float Health { get { return this.currentHealth; } }
  public bool Invulnerable { get { return this.invulnerable; } }
  public Vector2 DamageDirection { get { return this.damageDirection; } }
  public float InvulnerabilityDuration { get { return this.invulnerabilityDuration; } }

  private void Awake() {
    this.currentHealth = this.maxHealth;
  }

  protected virtual void OnEnable() {
    if (this.resetOnEnable) {
      this.currentHealth = this.maxHealth;
    }
    this.invulnerable = false;
  }

  protected virtual void Update() {
    if (invulnerable) {
      invulnerableTimer -= Time.deltaTime;
      if (invulnerableTimer <= 0f) {
        this.invulnerable = false;
      }
    }
  }

  public void DisableInvlnerability() {
    this.invulnerable = false;
    this.invulnerableTimer = 0f;
  }

  public void EnableInvulnerability(bool noTimer = false) {
    this.invulnerable = true;
    this.invulnerableTimer = noTimer ? float.MaxValue : this.invulnerabilityDuration;
  }

  public void TakeDamage(DamageDealer dealer, bool ignoreInvincible = false) {
    if ((this.invulnerable && !ignoreInvincible) || currentHealth <= 0) {
      return;
    }

    if (!invulnerable) {
      this.currentHealth -= dealer.Damage;
      this.EnableInvulnerability();
    }

    this.damageDirection = this.transform.position - dealer.transform.position;
    this.OnTakeDamage.Invoke(dealer, this);

    if (this.currentHealth <= 0) {
      this.OnDie.Invoke(dealer, this);
      if (this.disableOnDeath) {
        this.gameObject.SetActive(false);
      }
    }
  }

  public void GainHealth(float amount) {
    this.currentHealth += amount;
    if (this.currentHealth > this.maxHealth) {
      this.currentHealth = this.maxHealth;
    }
    OnGainHealth.Invoke(amount, this);
  }
}