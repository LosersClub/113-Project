using System;
using UnityEngine;

public class Action {
  private IInputDevice binding;
  private string name;
  public delegate void ActionToExecute();
  private ActionToExecute boundAction;

  public Action(string name, ActionToExecute boundAction, IInputDevice binding) {
    this.name = name;
    this.boundAction = boundAction;
    this.binding = binding;
  }

  public event ActionToExecute BoundAction
  {
    add { boundAction += value; }
    remove { boundAction -= value; }
  }

  public IInputDevice InputDevice {
    get { return binding; }
    set { binding = value; }
  }

  public string Name {
    get { return name; }
    set { name = value; }
  }

  public void Update() {
    // binding is to a valid and active mouse button
    if (binding.InputActive()) {
      boundAction();
    }
  }

}