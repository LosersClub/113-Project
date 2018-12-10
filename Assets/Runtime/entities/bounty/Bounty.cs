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
  public GameObject scythePrefab;
  public GameObject spike;
  public float spikeSpacing = 4.5f;

  public float burrowSpeed = 5f;
  public float burrowOutSpeed = 10f;
  public float burrowDuration = 3.5f;

  public int burrowCooldown = 3;
  public int seedsCooldown = 3;
  public float burrowProb = 0.50f;
  public float seedsProb = 0.25f;

  [HideInInspector]
  public BoxCollider2D hitbox;
  
  private MovementController movement;
  private MeleeDamageDealer meleeDealer;
  private Animator animator;
  private SpriteRenderer sprite;
  private Player player;
  private GameObject spikeManager;
  private GameObject scythe;

  private bool active = false;
  private bool inGround = false;
  private int curBurrowCooldown = 0;
  private int curSeedsCooldown = 0;
  private int meleeCount = 0;

  #region Animator Variables
  private readonly int meleeParam = Animator.StringToHash("melee");
  private readonly int seedsParam = Animator.StringToHash("seeds");
  private readonly int burrowParam = Animator.StringToHash("burrow");
  private readonly int endBurrowParam = Animator.StringToHash("endBurrow");
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
    this.hitbox = this.GetComponent<BoxCollider2D>();
    this.player = GameManager.Player;

    this.spikeManager = new GameObject("Spike Manager");
    this.spikeManager.transform.position = Vector3.zero;

    this.scythe = Instantiate(this.scythePrefab);
    this.scythe.SetActive(false);

    for (int i = 0; i < 35/this.spikeSpacing; i++) {
      GameObject newSpike = Instantiate(this.spike);
      newSpike.transform.SetParent(this.spikeManager.transform);
      newSpike.transform.position = new Vector2(2.5f + i * this.spikeSpacing, 1.5f);
      newSpike.SetActive(false);
    }
  }

  // TODO: Coroutine to active when player is within range

  private void Update() {
    if (!inGround) {
      this.movement.Velocity.y -= this.gravity * Time.deltaTime;
      this.movement.Move(this.movement.Velocity * Time.deltaTime);
    }
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

  public void Seeds() {
    foreach (Transform child in this.spikeManager.transform) {
      child.gameObject.SetActive(true);
    }
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

  public void BurrowMove() {
    this.StartCoroutine(this.BurrowCoroutine());
  }

  public void BurrowOut() {
    this.StartCoroutine(this.BurrowOutCoroutine());
  }

  private IEnumerator BurrowOutCoroutine() {
    this.inGround = true;
    this.hitbox.enabled = true;
    this.transform.position = new Vector2(this.transform.position.x, -1.5f);
    float timer = 4 / this.burrowOutSpeed;
    while (timer > 0) {
      timer -= Time.deltaTime;
      this.transform.Translate(new Vector3(0, burrowOutSpeed * Time.deltaTime), Space.World);
      yield return null;
    }
    this.inGround = false;
  }

  private IEnumerator BurrowCoroutine() {
    float timer = this.burrowDuration;
    while (timer > 0) {
      timer -= Time.deltaTime;
      float target = this.player.transform.position.x - this.transform.position.x;
      target /= Mathf.Abs(target);
      this.movement.Velocity.x = Mathf.Lerp(this.movement.Velocity.x, this.burrowSpeed * target, 1f * Time.deltaTime);
      yield return null;
    }
    this.movement.Velocity.x = 0;
    yield return new WaitForSeconds(0.2f);
    this.animator.SetTrigger(this.endBurrowParam);
  }

  private IEnumerator NextAction() {
    while (!active) {
      if (this.transform.position.x - this.player.transform.position.x <= 8f) {
        this.active = true;
      }
      yield return null;
    }
    yield return new WaitForSeconds(0.5f);
    bool action = false;
    while (!action) {
      //yield return new WaitForSeconds(1f);
      if (this.curBurrowCooldown == 0 && Random.value <= this.burrowProb) {
        this.animator.SetTrigger(this.burrowParam);
        this.meleeCount = 0;
        this.curBurrowCooldown = this.burrowCooldown;
        action = true;
      } else if (this.curSeedsCooldown == 0 && Random.value <= this.seedsProb) {
        this.animator.SetTrigger(this.seedsParam);
        this.meleeCount = 0;
        this.curSeedsCooldown = this.seedsCooldown;
        action = true;
      } else if (this.meleeCount < 3) {
        this.animator.SetTrigger(this.meleeParam);
        this.meleeCount++;
        action = true;
      }
    }
  }

  public void DetermineNextAction() {
    this.StartCoroutine(this.NextAction());
  }

  public void ReduceCooldowns() {
    if (this.curBurrowCooldown > 0) {
      this.curBurrowCooldown--;
    }
    if (this.curSeedsCooldown > 0) {
      this.curSeedsCooldown--;
    }
  }
}