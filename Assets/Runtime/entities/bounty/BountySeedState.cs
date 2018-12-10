using UnityEngine;

public class BountySeedState : SceneLinkedState<Bounty> {
  public override void OnEnter(Animator animator, AnimatorStateInfo info, int layerIndex) {
    this.instance.Seeds();
  }
}