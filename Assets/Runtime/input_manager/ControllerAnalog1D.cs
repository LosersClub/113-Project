using UnityEngine;
using XInputWrapper;

/* 
 * This class represents a trigger (1D analog) on a controller.
 */
public class ControllerAnalog1D : IAnalog1D
{
  private Side side;

  public ControllerAnalog1D(Side side) {
    this.side = side;
  }

  public Side Binding {
    get { return side; }
    set { side = value; }
  }

  // Returns the float representing how far the trigger is pressed
  public float Axis() {
    // For now, assume user index is 0
    ControllerState state = ControllerInput.GetState(0);
    return state.TriggerStatus(side);
  }
}