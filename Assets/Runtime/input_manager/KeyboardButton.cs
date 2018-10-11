using UnityEngine;

/* 
 * This class represents a keyboard button.
 */
public class KeyboardButton : IButton {
  private KeyCode binding;

  public KeyboardButton(KeyCode key) {
    this.binding = key;
  }

  public KeyCode Binding {
    get { return binding; }
    set { binding = value; }
  }

  // Returns whether the button is currently pressed
  public bool InputActive() {
    return Input.GetKey(binding);
  }
}