using UnityEngine;
using XInputWrapper;

/* 
 * This class represents a button on a controller.
 */
public class ControllerButton : IButton {
  private Button button;

  public ControllerButton(Button button) {
    this.button = button;
  }

  public Button Binding {
    get { return button; }
    set { button = value; }
  }

  // Returns whether the button is currently pressed
  public bool InputActive() {
    // For now, we assume user index = 0.
    ControllerState state = ControllerInput.GetState(0);
    return state.ButtonStatus(button);
  }
}