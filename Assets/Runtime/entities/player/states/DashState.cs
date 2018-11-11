using UnityEngine;

public class DashState : SceneLinkedState<Player> {
  public override void OnEnter(Animator animator, AnimatorStateInfo info, int layerIndex) {
    this.instance.SetCrouching();
    this.instance.StartDash();
  }

  public override void OnExit(Animator animator, AnimatorStateInfo info, int layerIndex) {
    this.instance.SetStanding();
  }

  public override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //this.instance.Reset();
  }
}