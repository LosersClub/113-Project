using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class Bounty : MonoBehaviour {

  public float gravity = 80f;

  public Probabilities probs;

  [Serializable]
  public class Probabilities {
    [Range(0f, 1f)]
    public float melee = 0.5f;
  }

  public float dashSpeed = 15f;
  public float scytheSpeed = 10f;

  public Vector2 slashPosition;
  public ParticleSystem slash;
  public Vector2 scythePosition;
  public GameObject scythe;

  private MovementController movement;
  private MeleeDamageDealer meleeDealer;
  private Animator animator;
  private SpriteRenderer sprite;
  private Player player;

  private bool active = true; // TODO: false
  private int meleeCount = 0;

  #region Animator Variables
  private readonly int meleeParam = Animator.StringToHash("melee");
  private readonly int dashParam = Animator.StringToHash("dashFinished");
  private readonly int endThrowParam = Animator.StringToHash("endThrow");
  private readonly int killParam = Animator.StringToHash("kill");
  #endregion

  public bool FacingRight { get { return this.sprite.flipX; } }

  private void Awake() {
    this.movement = this.GetComponent<MovementController>();
    this.meleeDealer = this.GetComponent<MeleeDamageDealer>();
    this.sprite = this.GetComponent<SpriteRenderer>();
    this.animator = this.GetComponent<Animator>();
    this.player = GameManager.Player;
  }

  // TODO: Coroutine to active when player is within range

  private void Update() {
    this.movement.Velocity.y -= this.gravity * Time.deltaTime;
    this.movement.Move(this.movement.Velocity * Time.deltaTime);
  }

  public void UpdateFacing() {
    float delta = player.transform.position.x - this.transform.position.x;
    if (delta > 0f) {
      this.sprite.flipX = true;
    } else if (delta < 0f) {
      this.sprite.flipX = false;
    }
  }

  public void Dash() {
    this.StartCoroutine(this.DashCoroutine());
  }

  public Action MeleeDirection() {
    this.Slash(this.FacingRight);
    return () => this.meleeDealer.HorizontalHit(this.FacingRight);
  }

  private void Slash(bool right) {
    this.slash.transform.localScale = new Vector3(right ? -1 : 1, 1, 1);
    this.slash.transform.localPosition = new Vector2(
      (right ? 1 : -1) * this.slashPosition.x, this.slashPosition.y);
    this.slash.Play();
  }

  public void ThrowScythe() {
    this.scythe.transform.position = this.transform.position + new Vector3(
      (this.FacingRight ? 1 : -1) * this.scythePosition.x, this.scythePosition.y);
    this.scythe.SetActive(true);

    Vector2 direction = this.player.transform.position - this.scythe.transform.position;
    float dist = direction.magnitude;
    direction = direction / dist;
    this.StartCoroutine(ScytheMove(direction, dist));
  }

  private IEnumerator ScytheMove(Vector2 direction, float distance) {
    float distTravelled = 0f;
    bool triggered = false;
    while (distTravelled < distance) {
      float dist = this.scytheSpeed * Time.deltaTime;
      this.scythe.GetComponent<MovementController>().Move(direction * dist);
      distTravelled += dist;

      if (!triggered && distTravelled >= distance / 2) {
        this.animator.SetTrigger(this.endThrowParam);
        triggered = true;
      }
      yield return null;
    }
    this.scythe.GetComponent<Animator>().SetTrigger(this.killParam);
    float timer = 0.267f;
    while (timer > 0) {
      timer -= Time.deltaTime;
      this.scythe.GetComponent<MovementController>().Move(direction * this.scytheSpeed * Time.deltaTime);
      yield return null;
    }
    this.scythe.SetActive(false);
  }

  private IEnumerator DashCoroutine() {
    float timer = (Mathf.Abs(this.player.transform.position.x - this.transform.position.x) - 2f) / this.dashSpeed;
    this.movement.Velocity.x = (this.FacingRight ? 1 : -1) * this.dashSpeed;
    while (timer > 0) {
      timer -= Time.deltaTime;
      if (Math.Abs(this.movement.Velocity.x) < this.dashSpeed - 1f) {
        break;
      }
      yield return null;
    }
    while (Math.Abs(this.movement.Velocity.x) > 0.1f) {
      this.movement.Velocity.x = Mathf.Lerp(this.movement.Velocity.x, 0f, 40f * Time.deltaTime);
      yield return null;
    }
    this.movement.Velocity.x = 0;
    this.animator.SetTrigger(this.dashParam);
  }

  public void DetermineNextAction() {
    if (!active) {
      return;
    }

    this.animator.SetTrigger(this.meleeParam);
  }
}