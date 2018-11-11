using UnityEngine;

public class MeleeActionState : SceneLinkedState<Player> {
  public override void OnEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    this.instance.SetEyeColor(this.instance.EyeColor.melee);
    // SetMeleeDirection
  }

  public override void OnExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    this.instance.SetEyeColor(this.instance.EyeColor.original);
    // StartMeleeCooldown
  }

  public override void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    this.instance.HorizontalMovement(this.instance.GroundDamping, this.instance.MeleeScalar);
    this.instance.UpdateMelee();
    this.instance.Reset();
  }
}