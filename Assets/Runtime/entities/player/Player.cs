using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(MeleeDamageDealer))]
[RequireComponent(typeof(DamageTaker))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour {
  [Header("Movement Settings")]
  [SerializeField]
  private float gravity = 80f;
  [SerializeField]
  private float runSpeed = 8f;
  [SerializeField]
  [Range(0f, 1f)]
  private float crouchModifier = 0.8f;
  [SerializeField]
  [Range(0f, 1f)]
  private float meleeModifier = 0.5f;
  [SerializeField]
  private float groundDamping = 40f;

  [Header("Airborne Settings")]
  [SerializeField]
  private float airDamping = 20f;
  [SerializeField]
  private float minJumpHeight = 1f;
  [SerializeField]
  private float maxJumpTime = 0.3f;

  [Header("Dash Settings")]
  [SerializeField]
  private float dashSpeed = 25f;
  [SerializeField]
  private float dashDistance = 6f;
  [SerializeField]
  private float dashCooldown = 1f;

  [Header("Attack Settings")]
  [SerializeField]
  private PlayerBulletSpawner bulletSpawner;
  [SerializeField]
  private float rangedEyeDuration = 0.5f;
  [SerializeField]
  private float meleeBounceHeight = 2f;
  [SerializeField]
  [Range(0f, 1f)]
  private float meleeDirectionZone = 0.7f;

  [Space]
  [SerializeField]
  private HitboxData boxData;
  [SerializeField]
  private EyeColors eyeColors;

  [Header("Misc Settings")]
  [SerializeField]
  private float standRoom = 0.75f;
  [SerializeField]
  private int numBlinks = 10;

  [Serializable]
  private class HitboxData {
    public Vector2 standingOffset = new Vector2(0f, -0.06f);
    public Vector2 standingSize = new Vector2(0.625f, 1.87f);
    public Vector2 crouchOffset = new Vector2(0f, -0.41f);
    public Vector2 crouchSize = new Vector2(0.625f, 1.17f);
  }

  [Serializable]
  public class EyeColors {
    public Color original = new Color(1, 1, 0);
    public Color melee = new Color(0.717f, 0.015f, 0.847f);
    public Color ranged = new Color(0f, 0.788f, 0.098f);
    public Color magic = new Color(0f, 0.949f, 1f);
  }

  private MovementController controller;
  private MeleeDamageDealer meleeAttack;
  private DamageTaker damageTaker;
  private new BoxCollider2D collider;
  private SpriteRenderer sprite;
  private Animator animator;

  private float jumpTimer = 0f;
  private Vector2 movement = Vector2.zero;
  private bool jumping = false;
  private bool meleeHeld = false;
  private bool shootHeld = false;
  private int alternator = 0;
  private bool dashHeld = false;
  private bool canMelee = true;
  private bool canDash = true;
  private bool canShoot = true;

  #region Animator Variables
  private readonly int horizontalSpeedParam = Animator.StringToHash("HorizontalSpeed");
  private readonly int moveYParam = Animator.StringToHash("MoveY");
  private readonly int verticalSpeedParam = Animator.StringToHash("VerticalSpeed");
  private readonly int groundedParam = Animator.StringToHash("Grounded");
  private readonly int crouchingParam = Animator.StringToHash("Crouching");
  private readonly int dashingParam = Animator.StringToHash("Dashing");
  private readonly int meleeParam = Animator.StringToHash("Melee");
  private readonly int alternatorParam = Animator.StringToHash("Alternator");

  private readonly int eyeColorId = Shader.PropertyToID("_OutputColor");
  #endregion

  #region Properties
  public float GroundDamping { get { return this.groundDamping; } }
  public float AirDamping { get { return this.airDamping; } }
  public float CrouchScalar { get { return this.crouchModifier; } }
  public float MeleeScalar { get { return this.meleeModifier; } }
  public EyeColors EyeColor { get { return this.eyeColors; } }
  #endregion

  #region Unity Methods
  private void Awake() {
    this.controller = this.GetComponent<MovementController>();
    this.collider = this.GetComponent<BoxCollider2D>();
    this.sprite = this.GetComponent<SpriteRenderer>();
    this.animator = this.GetComponent<Animator>();
    this.meleeAttack = this.GetComponent<MeleeDamageDealer>();
    this.damageTaker = this.GetComponent<DamageTaker>();
  }

  private void Start() {
    SceneLinkedState<Player>.Initialize(this.animator, this);
    this.SetStanding();
    this.SetEyeColor(this.eyeColors.original);
    this.meleeAttack.OnDamageHit.AddListener(this.BounceOnDownHit);
    this.damageTaker.OnTakeDamage.AddListener(this.BlinkOnHit);
  }

  private void Update() {
    this.controller.Move(this.controller.Velocity * Time.deltaTime);
    this.animator.SetFloat(this.horizontalSpeedParam, this.controller.Velocity.x);
    this.animator.SetFloat(this.verticalSpeedParam, this.controller.Velocity.y);
    this.animator.SetFloat(this.moveYParam, this.movement.y);
    this.animator.SetBool(this.groundedParam, this.controller.Grounded);
  }
  #endregion

  public void Reset() {
    if (!this.meleeHeld) {
      this.canMelee = true;
    }
    if (!this.shootHeld) {
      this.canShoot = true;
    }
    this.movement = Vector2.zero;
    this.jumping = false;
    this.meleeHeld = false;
    this.dashHeld = false;
    this.shootHeld = false;
  }

  public void UpdateFacing() {
    if (this.movement.x > 0f) {
      this.sprite.flipX = false;
    } else if (this.movement.x < 0f) {
      this.sprite.flipX = true;
    }
  }

  public void SetStanding() {
    this.collider.offset = this.boxData.standingOffset;
    this.collider.size = this.boxData.standingSize;
    this.controller.RecalculateRaySpacing();
    this.animator.SetBool(this.crouchingParam, false);
  }

  public void SetCrouching() {
    this.collider.offset = this.boxData.crouchOffset;
    this.collider.size = this.boxData.crouchSize;
    this.controller.RecalculateRaySpacing();
    this.animator.SetBool(this.crouchingParam, true);
  }

  private bool SpaceToStand() {
    Bounds inset = this.collider.bounds;
    Vector2 origin = new Vector2(inset.min.x + this.controller.Inset, inset.max.y - this.controller.Inset);
    Vector2 ray;
    for (int i = 0; i < this.controller.VerticalRays; i++) {
      ray = new Vector2(origin.x + i * this.controller.Spacing.x, origin.y);
      Rays.DrawRay(ray, Vector2.up, this.standRoom, Color.blue);
      if (Rays.IsHitting(ray, Vector2.up, this.standRoom, this.controller.Ground)) {
        return false;
      }
    }
    return true;
  }

  public void SetEyeColor(Color color) {
    this.sprite.material.SetColor(eyeColorId, color);
  }

  public Action SetMeleeDirection() {
    if (this.movement.y > this.meleeDirectionZone) {
      return () => this.meleeAttack.VerticalHit(true);
    } else if (this.movement.y < -this.meleeDirectionZone) {
      return () => this.meleeAttack.VerticalHit(false);
    } else if (this.sprite.flipX) {
      return () => this.meleeAttack.HorizontalHit(false);
    }
    return () => this.meleeAttack.HorizontalHit(true);
  }

  public void HorizontalMovement(float damping, float scale = 1.0f) {
    float targetSpeed = Mathf.Clamp(this.movement.x, -1.0f, 1.0f) * this.runSpeed * scale;
    this.controller.Velocity.x = Mathf.Lerp(this.controller.Velocity.x, targetSpeed,
      damping * Time.deltaTime);
  }

  public void VerticalMovement() {
    this.controller.Velocity.y -= gravity * Time.deltaTime;
  }

  public void UpdateJump() {
    if (this.jumpTimer <= 0 || !this.jumping || this.controller.Collision.Above) {
      this.jumpTimer = 0;
      return;
    }
    this.controller.Velocity.y = Mathf.Sqrt(2f * this.minJumpHeight * this.gravity);
    this.jumpTimer -= Time.deltaTime;
  }

  public void CheckForIgnorePlatform() {
    if (this.jumping && this.movement.y < 0) {
      this.controller.IgnorePlatforms = true;
    }
  }

  public bool CheckForStand() {
    if (this.movement.y >= 0 && this.SpaceToStand()) {
      this.animator.SetBool(this.crouchingParam, false);
      return true;
    }
    return false;
  }

  public bool CheckForJump() {
    if (this.jumping) {
      this.jumpTimer = maxJumpTime;
      UpdateJump();
      return true;
    }
    return false;
  }

  public bool CheckForMelee() {
    if (this.meleeHeld && this.canMelee && !this.damageTaker.Invulnerable) {
      this.canMelee = false;
      this.animator.SetTrigger(this.meleeParam);
      this.animator.SetFloat(this.alternatorParam, this.alternator = (this.alternator + 1) % 2);
      return true;
    }
    return false;
  }

  public void CheckForRanged() {
    if (this.shootHeld && this.canShoot && !this.damageTaker.Invulnerable) {
      this.canShoot = false;
      this.bulletSpawner.SpawnBullet();
      this.StartCoroutine(this.RangeEyeCoroutine());
    }
  }

  public bool CheckForDash() {
    if (this.dashHeld && this.canDash) {
      this.canDash = false;
      this.animator.SetBool(this.dashingParam, true);
      return true;
    }
    return false;
  }

  public void StartDash() {
    this.jumpTimer = 0;
    this.controller.Velocity.y = 0;
    this.StartCoroutine(this.DashCoroutine());
  }

  private IEnumerator Blink(float duration) {
    float blinkDuration = duration / (numBlinks * 2);
    for (float timer = duration; timer > 0;) {
      timer -= Time.deltaTime;
      this.sprite.enabled = !this.sprite.enabled;
      yield return new WaitForSeconds(blinkDuration);
      timer -= blinkDuration;
    }
    this.sprite.enabled = true;
  }

  private IEnumerator RangeEyeCoroutine() {
    this.SetEyeColor(this.eyeColors.ranged);
    yield return new WaitForSeconds(this.rangedEyeDuration);
    this.SetEyeColor(this.eyeColors.original);
  }

  private IEnumerator DashCoroutine() {
    this.canDash = false;
    this.controller.Velocity.x = (this.sprite.flipX ? -1 : 1) * this.dashSpeed;
    yield return new WaitForSeconds(this.dashDistance / this.dashSpeed);
    this.animator.SetBool(this.dashingParam, false);
    yield return new WaitForSeconds(this.dashCooldown);
    this.canDash = true;
  }

  private void BounceOnDownHit(DamageDealer dealer, DamageTaker taker) {
    if (((MeleeDamageDealer)dealer).HitDirection == Vector2.down) {
      this.controller.Velocity.y = Mathf.Sqrt(2f * this.meleeBounceHeight * this.gravity);
    }
  }

  private void BlinkOnHit(DamageDealer dealer, DamageTaker taker) {
    this.StartCoroutine(this.Blink(taker.InvulnerabilityDuration));
  }

  #region Input
  public void Move(float x, float y) {
    if (Mathf.Abs(x) > 0.3f) {
      this.movement.x += x;
    }
    if (Mathf.Abs(y) > 0.3f) {
      this.movement.y += y;
    }
  }

  public void JoystickAim(float x, float y) {
    this.bulletSpawner.UpdatePosition(x, y);
  }

  public void Shoot() {
    this.shootHeld = true;
  }

  public void Jump() {
    this.jumping = true;
  }

  public void Melee() {
    this.meleeHeld = true;
  }

  public void Dash() {
    this.dashHeld = true;
  }
  #endregion
}