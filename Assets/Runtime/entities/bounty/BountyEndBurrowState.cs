using UnityEngine;

public class BountyEndBurrowState : SceneLinkedState<Bounty> {
  public override void OnEnter(Animator animator, AnimatorStateInfo info, int layerIndex) {
    this.instance.BurrowOut();
  }
}