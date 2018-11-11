using UnityEngine;

public class LocomotionState : SceneLinkedState<Player> {
  public override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    this.instance.UpdateFacing();
    this.instance.HorizontalMovement(this.instance.GroundDamping);
    this.instance.VerticalMovement();

    if (this.instance.CheckForJump() || this.instance.CheckForDash() || this.instance.CheckForMelee()) { }
    this.instance.Reset();
  }
}