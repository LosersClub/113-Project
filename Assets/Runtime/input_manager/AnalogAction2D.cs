using System;
using UnityEngine;
using XInputWrapper;

public class AnalogAction2D : IAnalogAction {

  private IAnalog2D binding;
  private string name;
  public delegate void ActionToExecute(float x, float y);
  private ActionToExecute boundAction;

  public AnalogAction2D(string name, ActionToExecute boundAction, IAnalog2D binding) {
    this.name = name;
    this.boundAction = boundAction;
    this.binding = binding;
  }

  public event ActionToExecute BoundAction {
    add { boundAction += value; }
    remove { boundAction -= value; }
  }

  public IAnalog2D InputDevice {
    get { return binding; }
    set { binding = value; }
  }

  public string Name {
    get { return name; }
    set { name = value; }
  }

  public void Update() {
    Thumbstick axes = binding.Axes();
    boundAction(axes.X, axes.Y);
  }

  public int Rebind(IInputDevice binding) {
    if (binding is IAnalog2D) {
      this.binding = (IAnalog2D)binding;
      return 1;
    }
    return 0;
  }
}