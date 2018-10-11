using UnityEngine;

/* 
 * This class represents a mouse button.
 */
public class MouseButton : IButton {
  // 0 = left button, 1 = right button, 2 = middle button
  private int binding;

  public MouseButton(int buttonNumber) {
    this.binding = buttonNumber;
  }

  public int Binding
  {
    get { return binding; }
    set { if (binding <= 0 && binding >= 2) binding = value; }
  }

  // Returns whether the button is currently pressed
  public bool InputActive()
  {
    return Input.GetMouseButton(binding);
  }
}