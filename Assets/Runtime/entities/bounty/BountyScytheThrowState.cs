using UnityEngine;

public class BountyScytheThrowState : SceneLinkedState<Bounty> {
  public override void OnEnter(Animator animator, AnimatorStateInfo info, int layerIndex) {
    this.instance.ThrowScythe();
  }
}