using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(MovementController))]
public class Chalice : MonoBehaviour {

  private enum State {Init, MoveWait, Moving, FireTell, Fire};

  // TODO: refactor state machine code into a separate StandaloneStateMachine class.
  // Note: should use ReadOnlyDictionary instead, but that is only available in .NET 4 (while we
  // are using .NET 3.5):
  private static readonly Dictionary<State, HashSet<State>> StateTransitions = new Dictionary<State, HashSet<State>> {
    {State.Init, new HashSet<State>{State.MoveWait}},
    {State.MoveWait, new HashSet<State>{State.Moving, State.FireTell}},
    {State.Moving, new HashSet<State>{State.MoveWait}},
    {State.FireTell, new HashSet<State>{State.Fire}},
    {State.Fire, new HashSet<State>{State.MoveWait}}
  };

  [SerializeField]
  private GameObject beamEmitterObject;
  [SerializeField]
  private float firingRange = 4f;
  [SerializeField]
  private float moveSpeed = 0.2f;
  // During a MovingState, attempts to move the following distance. May move
  // less if stopped by a barrier:
  [SerializeField]
  private float moveDistance = 1f;

  private SpriteRenderer spriteRendererComponent;
  private MovementController movementController;

  private BeamEmitter beamEmitterComponent;
  private State state = State.Init;
  private Vector2 moveDirection = Vector2.zero; // will be reassigned before movement

  void Start () {
    Assert.IsNotNull(this.beamEmitterObject);

    this.spriteRendererComponent = this.GetComponent<SpriteRenderer>();
    this.movementController = this.GetComponent<MovementController>();

    this.beamEmitterComponent = this.beamEmitterObject.GetComponent<BeamEmitter>();
    Assert.IsNotNull(this.beamEmitterComponent);

    this.ChangeState(State.MoveWait);
  }

  void Update () {
    if(this.state == State.FireTell) {
      this.spriteRendererComponent.color = Color.yellow;
    }
    else if(this.state == State.Fire) {
      this.spriteRendererComponent.color = Color.blue;
    }
    else {
      this.spriteRendererComponent.color = Color.white;

      if(this.state == State.Moving) {
        this.movementController.Move(this.moveSpeed * Time.deltaTime * this.moveDirection);
      }
    }
  }

  private void AimAtPlayer() {
    Vector2 direction = GameManager.Player.transform.position - this.transform.position;
    direction.Normalize();
    Vector3 directionUpwards = Vector3.Cross(new Vector3(direction.x, direction.y, 0), -Vector3.forward);
    this.transform.rotation = Quaternion.LookRotation(Vector3.forward, directionUpwards);
  }

  private void ChangeState(State newState) {
    if(!Chalice.StateTransitions[this.state].Contains(newState)) {
      throw new ArgumentException(String.Format("Cannot transition from State {0} to State {1}",
                                                this.state, newState));
    }

    // State exit actions:
    // None currently.

    // State entry actions:
    if(newState == State.MoveWait) {
      StartCoroutine(MoveWaitCoroutine());
    }
    else if(newState == State.Moving) {
      StartCoroutine(MoveThenStopCoroutine());
    }
    else if(newState == State.FireTell) {
      StartCoroutine(AimThenFireCoroutine());
    }
    else if(newState == State.Fire) {
      StartCoroutine(FireThenMoveStateCoroutine());
    }

    this.state = newState;
  }

  private IEnumerator AimThenFireCoroutine() {
    this.AimAtPlayer();
    yield return new WaitForSeconds(1);
    this.ChangeState(State.Fire);
  }

  private IEnumerator FireThenMoveStateCoroutine() {
    this.beamEmitterComponent.Fire();
    yield return new WaitUntil(() => !this.beamEmitterComponent.IsFiring);
    this.ChangeState(State.MoveWait);
  }

  private IEnumerator MoveWaitCoroutine() {
    yield return new WaitForSeconds(1);
    if(this.InFiringRange()) {
      this.ChangeState(State.FireTell);
    }
    else {
      this.moveDirection = (GameManager.Player.transform.position - this.transform.position).normalized;
      this.ChangeState(State.Moving);
    }
  }

  private IEnumerator MoveThenStopCoroutine() {
    // Wait, as Movement is performed in update:
    yield return new WaitForSeconds(this.moveDistance / this.moveSpeed);
    this.ChangeState(State.MoveWait);
  }

  private bool InFiringRange() {
    return (GameManager.Player.transform.position - this.transform.position).magnitude <= this.firingRange;
  }
}
