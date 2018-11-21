using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ArcherArrow : MonoBehaviour {

  private enum State {Init, Fired, HitDamageTaker, HitImpassable, ToBeDestroyed};

  // Note: should use ReadOnlyDictionary instead, but that is only available in .NET 4 (while we
  // are using .NET 3.5):
  private static readonly Dictionary<State, HashSet<State>> StateTransitions = new Dictionary<State, HashSet<State>> {
    {State.Init, new HashSet<State>{State.Fired}},
    {State.Fired, new HashSet<State>{State.HitDamageTaker, State.HitImpassable}},
    {State.HitDamageTaker, new HashSet<State>{State.ToBeDestroyed}},
    {State.HitImpassable, new HashSet<State>{State.ToBeDestroyed}},
    {State.ToBeDestroyed, new HashSet<State>()}
  };

  [SerializeField]
  private float speed = 1f;

  private Rigidbody2D rigidBody2D;

  private State state = State.Init;
  private Vector2 direction = new Vector2(1, 0);

  void Start () {
    this.rigidBody2D = this.GetComponent<Rigidbody2D>();
  }

  void Update () {

  }

  void FixedUpdate() {
    if(this.state == State.Fired) {
      this.rigidBody2D.MovePosition(this.rigidBody2D.position + Time.deltaTime * speed * direction);
    }
  }

  public void Fire() {
    this.ChangeState(State.Fired);
  }

  private void ChangeState(State newState) {
    if(!ArcherArrow.StateTransitions[this.state].Contains(newState)) {
      throw new ArgumentException(String.Format("Cannot transition from State {0} to State {1}",
                                                this.state, newState));
    }

    this.state = newState;
  }
}
