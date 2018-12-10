using UnityEngine;

public class BountySpikeDeadState : SceneLinkedState<BountySpike> {
  public override void OnEnter(Animator animator, AnimatorStateInfo info, int layerIndex) {
    this.instance.DisableHitbox();
    this.instance.gameObject.SetActive(false);
  }
}