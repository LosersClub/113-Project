using UnityEngine;

public class BountyDashState : SceneLinkedState<Bounty> {
  public override void OnEnter(Animator animator, AnimatorStateInfo info, int layerIndex) {
    this.instance.Dash();
  }
}