using UnityEngine;

public class BountyMeleeState : SceneLinkedState<Bounty> {
  private System.Action attack;

  public override void OnEnter(Animator animator, AnimatorStateInfo info, int layerIndex) {
    this.attack = this.instance.MeleeDirection();
  }

  public override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    this.attack();
  }
}