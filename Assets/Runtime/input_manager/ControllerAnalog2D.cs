using UnityEngine;
using XInputWrapper;

/* 
 * This class represents a thumbstick (2D analog) on a controller.
 */
public class ControllerAnalog2D : IAnalog2D {
  private Side side;

  public ControllerAnalog2D(Side side) {
    this.side = side;
  }

  public Side Binding {
    get { return side; }
    set { side = value; }
  }

  // Returns the two floats representing the thumbstick axes
  public Thumbstick Axes() {
    // For now, assume user index is 0
    ControllerState state = ControllerInput.GetState(0);
    return state.ThumbstickStatus(side);
  }
}