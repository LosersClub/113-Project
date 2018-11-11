using UnityEngine;

public class AirborneState : SceneLinkedState<Player> {
  public override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    this.instance.UpdateFacing();
    this.instance.UpdateJump();
    this.instance.HorizontalMovement(this.instance.AirDamping);
    this.instance.VerticalMovement();
    this.instance.CheckForIgnorePlatform();
    this.instance.CheckForRanged();

    if (this.instance.CheckForDash() || this.instance.CheckForMelee()) { }
    this.instance.Reset();
  }
}