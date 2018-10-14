using System;
using UnityEngine;

public class ButtonAction : IAction {
  private IButton binding;
  private string name;
  public delegate void ActionToExecute();
  private ActionToExecute boundAction;

  public ButtonAction(string name, ActionToExecute boundAction, IButton binding) {
    this.name = name;
    this.boundAction = boundAction;
    this.binding = binding;
  }

  public event ActionToExecute BoundAction
  {
    add { boundAction += value; }
    remove { boundAction -= value; }
  }

  public IButton InputDevice {
    get { return binding; }
    set { binding = value; }
  }

  public string Name {
    get { return name; }
    set { name = value; }
  }

  public void Update() {
    if (binding.InputActive()) {
      boundAction();
    }
  }

  public int Rebind(IInputDevice binding) {
    if (binding is IButton) {
      this.binding = (IButton)binding;
      return 1;
    }
    return 0;
  }
}