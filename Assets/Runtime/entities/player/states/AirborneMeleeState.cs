using UnityEngine;
using System;

public class AirborneMeleeState : SceneLinkedState<Player> {
  private Action attack;

  public override void OnEnter(Animator animator, AnimatorStateInfo info, int layerIndex) {
    this.instance.SetEyeColor(this.instance.EyeColor.melee);
    this.attack = this.instance.SetMeleeDirection();
  }

  public override void OnExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    this.instance.SetEyeColor(this.instance.EyeColor.original);
    // StartMeleeCooldown
  }

  public override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    this.instance.UpdateJump();
    this.instance.HorizontalMovement(this.instance.AirDamping, this.instance.MeleeScalar);
    this.instance.VerticalMovement();
    this.attack();
    this.instance.CheckForIgnorePlatform();
    this.instance.CheckForRanged();
    this.instance.Reset();
  }
}