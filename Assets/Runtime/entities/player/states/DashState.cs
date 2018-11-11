using UnityEngine;

public class DashState : SceneLinkedState<Player> {
  public override void OnEnter(Animator animator, AnimatorStateInfo info, int layerIndex) {
    this.instance.SetCrouching();
    this.instance.StartDash();
  }

  public override void OnExit(Animator animator, AnimatorStateInfo info, int layerIndex) {
    if (this.instance.CheckForStand()) {
      this.instance.SetStanding();
    }
  }
}