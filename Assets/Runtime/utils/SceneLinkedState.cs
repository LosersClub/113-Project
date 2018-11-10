using UnityEngine;
using UnityEngine.Animations;

public class SceneLinkedState<T> : SealedState where T : MonoBehaviour {
  protected T instance;

  private bool firstFrameOccurred;
  private bool lastFrameOccurred;

  public static void Initialize(Animator animator, T instance) {
    foreach (SceneLinkedState<T> state in animator.GetBehaviours<SceneLinkedState<T>>()) {
      state.InstanceInitialize(animator, instance);
    }
  }

  private void InstanceInitialize(Animator animator, T instance) {
    this.instance = instance;
    this.OnStart(animator);
  }

  public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
      AnimatorControllerPlayable controller) {
#if UNITY_EDITOR
    // Unity destroys/adds state when you change properties, need to reset instance otherwise it'll crash
    if (this.instance == null) {
      this.instance = animator.GetComponent<T>();
    }
#endif

    this.firstFrameOccurred = false;
    this.OnEnter(animator, stateInfo, layerIndex);
  }

  public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
      AnimatorControllerPlayable controller) {
    this.lastFrameOccurred = false;
    this.OnExit(animator, stateInfo, layerIndex);
  }

  public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
      AnimatorControllerPlayable controller) {
    if (!animator.gameObject.activeSelf) {
      return;
    }

    if (!animator.IsInTransition(layerIndex)) {
      if (!this.firstFrameOccurred) {
        this.firstFrameOccurred = true;
        this.OnPostEnter(animator, stateInfo, layerIndex);
      } else {
        this.OnUpdate(animator, stateInfo, layerIndex);
      }
      return;
    }

    if (animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash) {
      this.OnTransitionToUpdate(animator, stateInfo, layerIndex);
    } else if (animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash) {
      this.OnTransitionFromUpdate(animator, stateInfo, layerIndex);
    } else if (!this.lastFrameOccurred && this.firstFrameOccurred) {
      this.lastFrameOccurred = true;
      this.OnPreExit(animator, stateInfo, layerIndex);
    }
  }

  public virtual void OnStart(Animator animator) { }
  public virtual void OnEnter(Animator animator, AnimatorStateInfo info, int layerIndex) { }
  public virtual void OnTransitionToUpdate(Animator animator, AnimatorStateInfo info, int layerIndex) { }
  public virtual void OnPostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
  public virtual void OnUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
  public virtual void OnPreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
  public virtual void OnTransitionFromUpdate(Animator animator, AnimatorStateInfo info, int layerIndex) { }
  public virtual void OnExit(Animator animator, AnimatorStateInfo info, int layerIndex) { }
}


public abstract class SealedState : StateMachineBehaviour {
  public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
  public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
  public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
}