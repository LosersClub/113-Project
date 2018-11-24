using UnityEngine;

public class DashState : SceneLinkedState<Player> {
  public override void OnEnter(Animator animator, AnimatorStateInfo info, int layerIndex) {
    this.instance.SetCrouching();
    this.instance.StartDash();
    this.instance.StartDashTrail();
    this.instance.DamageTaker.EnableInvulnerability(true);
  }

  public override void OnExit(Animator animator, AnimatorStateInfo info, int layerIndex) {
    if (this.instance.CheckForStand()) {
      this.instance.SetStanding();
    }
    this.instance.DamageTaker.DisableInvlnerability();
    this.instance.StopDashTrail();
  }

  public override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    this.instance.Reset();
  }
}