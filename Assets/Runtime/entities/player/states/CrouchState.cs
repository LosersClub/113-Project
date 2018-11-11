﻿using UnityEngine;

public class CrouchState : SceneLinkedState<Player> {
  public override void OnEnter(Animator animator, AnimatorStateInfo info, int layerIndex) {
    this.instance.SetCrouching();
  }

  public override void OnExit(Animator animator, AnimatorStateInfo info, int layerIndex) {
    this.instance.SetStanding();
  }

  public override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    this.instance.UpdateFacing();
    this.instance.HorizontalMovement(this.instance.GroundDamping, this.instance.CrouchScalar);
    this.instance.VerticalMovement();
    this.instance.CheckForIgnorePlatform();
    this.instance.CheckForStand();
    this.instance.Reset();
  }
}