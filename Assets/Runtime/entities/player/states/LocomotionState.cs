using UnityEngine;

public class LocomotionState : SceneLinkedState<Player> {

  public override void OnEnter(Animator animator, AnimatorStateInfo info, int layerIndex) {
    this.instance.StartRunTrail();
  }

  public override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    this.instance.UpdateFacing();
    this.instance.HorizontalMovement(this.instance.GroundDamping);
    this.instance.VerticalMovement();
    //this.instance.CheckForIgnorePlatform();
    this.instance.CheckForRanged();

    if (this.instance.CheckForJump() || this.instance.CheckForDash() || this.instance.CheckForMelee()) { }
    this.instance.Reset();
  }

  public override void OnExit(Animator animator, AnimatorStateInfo info, int layerIndex) {
    this.instance.StopRunTrail();
  }
}