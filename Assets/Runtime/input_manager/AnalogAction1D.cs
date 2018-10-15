using System;
using UnityEngine;

public class AnalogAction1D : IAnalogAction {
  private IAnalog1D binding;
  private string name;
  public delegate void ActionToExecute(float axis);
  private ActionToExecute boundAction;

  public AnalogAction1D(string name, ActionToExecute boundAction, IAnalog1D binding) {
    this.name = name;
    this.boundAction = boundAction;
    this.binding = binding;
  }

  public event ActionToExecute BoundAction {
    add { boundAction += value; }
    remove { boundAction -= value; }
  }

  public IAnalog1D InputDevice {
    get { return binding; }
    set { binding = value; }
  }

  public string Name {
    get { return name; }
    set { name = value; }
  }

  public void Update() {
    boundAction(binding.Axis());
  }

  public int Rebind(IInputDevice binding) {
    if (binding is IAnalog1D) {
      this.binding = (IAnalog1D)binding;
      return 1;
    }
    return 0;
  }
}