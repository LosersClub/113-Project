using UnityEngine;

public class BountySpikeGrowState : SceneLinkedState<BountySpike> {
  public override void OnEnter(Animator animator, AnimatorStateInfo info, int layerIndex) {
    this.instance.EnableHitbox();
  }
}