using UnityEngine;
using System.Collections.Generic;
using XInputWrapper;

public static class InputManager {
  private static Dictionary<string, IAction> registeredActions = new Dictionary<string, IAction>();

  public static void AddButtonAction(string name, ButtonAction.ActionToExecute boundAction, 
    IButton binding) {
    ButtonAction action = new ButtonAction(name, boundAction, binding);
    registeredActions.Add(name, action);
  }

  public static void AddAnalog1DAction(string name, AnalogAction1D.ActionToExecute boundAction,
    IAnalog1D binding) {
    AnalogAction1D action = new AnalogAction1D(name, boundAction, binding);
    registeredActions.Add(name, action);
  }

  public static void AddAnalog2DAction(string name, AnalogAction2D.ActionToExecute boundAction,
  IAnalog2D binding) {
    AnalogAction2D action = new AnalogAction2D(name, boundAction, binding);
    registeredActions.Add(name, action);
  }

  public static void RemoveAction(string name) {
    registeredActions.Remove(name);
  }

  public static void RebindAction(string name, IInputDevice binding) {
    registeredActions[name].Rebind(binding);
  }

  public static Vector3 GetMousePosition() {
    return Mouse.MousePosition();
  }

  public static void Update() {
    foreach (IAction action in registeredActions.Values) {
      // assume user index is 0 for now
      // this check makes sure that if it's an analog action that the controller is connected
      if (!(action is IAnalogAction && !ControllerInput.ControllerConnected(0))) {
        action.Update();
      }
    }
  }
}