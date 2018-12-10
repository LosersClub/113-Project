using UnityEngine;

public class BountyBurrowState : SceneLinkedState<Bounty> {
  public override void OnEnter(Animator animator, AnimatorStateInfo info, int layerIndex) {
    this.instance.hitbox.enabled = false;
    this.instance.BurrowMove();
  }
}