using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(DamageDealer))]
public class ArcherArrow : MonoBehaviour {

  private enum State {Init, Fired, HitDamageTaker, HitImpassable, FiredTimeout, ToBeDestroyed};

  // Note: should use ReadOnlyDictionary instead, but that is only available in .NET 4 (while we
  // are using .NET 3.5):
  private static readonly Dictionary<State, HashSet<State>> StateTransitions = new Dictionary<State, HashSet<State>> {
    {State.Init, new HashSet<State>{State.Fired}},
    {State.Fired, new HashSet<State>{State.HitDamageTaker, State.HitImpassable, State.FiredTimeout}},
    {State.HitDamageTaker, new HashSet<State>{State.ToBeDestroyed}},
    {State.HitImpassable, new HashSet<State>{State.ToBeDestroyed}},
    {State.FiredTimeout, new HashSet<State>{State.ToBeDestroyed}},
    {State.ToBeDestroyed, new HashSet<State>()}
  };

  [SerializeField]
  private float speed = 1f;
  [SerializeField]
  private Color impassableHitColor = Color.gray;
  [SerializeField]
  private float impassableHitDestroyDelay = 5f;
  [SerializeField]
  private float firedTimeout = 60f;

  private Rigidbody2D rigidBody2D;
  private DamageDealer damageDealer;

  private State state = State.Init;
  private Vector2 direction = Vector2.zero;

  void Start () {
    this.rigidBody2D = this.GetComponent<Rigidbody2D>();
    this.damageDealer = this.GetComponent<DamageDealer>();

    this.damageDealer.OnDamageHit.AddListener(this.OnDamageHit);
    this.damageDealer.OnNoDamageHit.AddListener(this.OnNoDamageHit);
  }

  void Update () {
    if(this.state == State.HitImpassable) {
      this.GetComponent<SpriteRenderer>().color = this.impassableHitColor;
    }
  }

  void FixedUpdate() {
    if(this.state == State.Fired) {
      Assert.AreNotEqual(Vector2.zero, this.direction);
      this.rigidBody2D.MovePosition(this.rigidBody2D.position + Time.deltaTime * speed * direction);
    }
  }

  public void Fire(Vector2 direction) {
    this.direction = direction.normalized;
    this.ChangeState(State.Fired);
  }

  private void OnDamageHit(DamageDealer dealer, DamageTaker taker) {
    if(this.state == State.Fired) {
      this.ChangeState(State.HitDamageTaker);
      // TODO: do something (e.g. animation) between hit and destroy states.
      // For now, immediately change to destroy state:
      this.ChangeState(State.ToBeDestroyed);
    }
  }

  private void OnNoDamageHit(DamageDealer dealer) {
    if(this.state == State.Fired) {
      this.ChangeState(State.HitImpassable);
    }
  }

  private IEnumerator ImpassableHitDestroyCoroutine() {
    yield return new WaitForSeconds(this.impassableHitDestroyDelay);
    this.ChangeState(State.ToBeDestroyed);
  }

  private IEnumerator FiredTimeoutCoroutine() {
    yield return new WaitForSeconds(this.firedTimeout);
    if(this.state == State.Fired) {
      this.ChangeState(State.FiredTimeout);
      // Note: uses FiredTimeout as intermediate state between Fired and ToBeDestroyed to guard
      // against bugs where transition directly from Fired to ToBeDestroyed.
      this.ChangeState(State.ToBeDestroyed);
    }
  }

  private void ChangeState(State newState) {
    if(!ArcherArrow.StateTransitions[this.state].Contains(newState)) {
      throw new ArgumentException(String.Format("Cannot transition from State {0} to State {1}",
                                                this.state, newState));
    }

    // State exit actions:
    // None currently.

    // State entry actions:
    if(newState == State.Fired) {
      StartCoroutine(FiredTimeoutCoroutine());
    }
    if(newState == State.HitDamageTaker || newState == State.HitImpassable) {
      this.damageDealer.CanDealDamage = false;

      if(newState == State.HitImpassable) {
        StartCoroutine(ImpassableHitDestroyCoroutine());
      }
    }
    else if(newState == State.ToBeDestroyed) {
      Destroy(this.gameObject);
    }

    this.state = newState;
  }
}
