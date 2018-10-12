using UnityEngine;
using System.Collections.Generic;

public static class InputManager {
  private static Dictionary<string, Action> registeredActions = new Dictionary<string, Action>();

  public static void AddAction(string name, Action.ActionToExecute boundAction, 
    IButton binding) {
    Action action = new Action(name, boundAction, binding);
    registeredActions.Add(name, action);

  }

  public static void RemoveAction(string name) {
    registeredActions.Remove(name);
  }

  public static void RebindAction(string name, IButton binding) {
    registeredActions[name].InputDevice = binding;
  }

  public static Vector3 GetMousePosition() {
    return Mouse.MousePosition();
  }

  public static void Update() {
    foreach (Action action in registeredActions.Values) {
      action.Update();
    }
  }
}