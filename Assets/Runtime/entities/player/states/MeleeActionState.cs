using UnityEngine;
using System;

public class MeleeActionState : SceneLinkedState<Player> {
  private Action attack;

  public override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    this.instance.SetEyeColor(this.instance.EyeColor.melee);
    this.attack = this.instance.SetMeleeDirection();
  }

  public override void OnExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    this.instance.SetEyeColor(this.instance.EyeColor.original);
    // StartMeleeCooldown
  }

  public override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    this.instance.HorizontalMovement(this.instance.GroundDamping, this.instance.MeleeScalar);
    this.attack();
    this.instance.CheckForRanged();
    this.instance.Reset();
  }
}