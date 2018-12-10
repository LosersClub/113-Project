using UnityEngine;

public class BountyIdleState : SceneLinkedState<Bounty> {
  public override void OnEnter(Animator animator, AnimatorStateInfo info, int layerIndex) {
    this.instance.DetermineNextAction();
  }

  public override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    this.instance.UpdateFacing();
  }

  public override void OnExit(Animator animator, AnimatorStateInfo info, int layerIndex) {
    this.instance.ReduceCooldowns();
  }
}