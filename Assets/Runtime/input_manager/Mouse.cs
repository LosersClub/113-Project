using UnityEngine;

/* This class represents the mouse. */
public class Mouse : IInputDevice {

  public bool InputActive() {
    return MouseWithinGameWindow(Input.mousePosition);
  }

  public static Vector3 MousePosition() {
    Vector3 position = Input.mousePosition;
    // Make sure the cursor is within the bounds of the game window
    if (!MouseWithinGameWindow(position)) {
      return new Vector3(-1, -1, -1);
    }
    return position;
  }

  private static bool MouseWithinGameWindow(Vector3 position) {
    return position.x < 0 || position.y < 0
      || position.x > Screen.width || position.y > Screen.height;
  }
  
}