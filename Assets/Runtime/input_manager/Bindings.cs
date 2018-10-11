using UnityEngine;

public class Bindings {
  public static void Initialize() {
    InputManager.AddAction("test", Bindings.exampleMethod, new KeyboardButton(KeyCode.Space));
    InputManager.AddAction("mouse test", Bindings.exampleMouseMethod, new MouseButton(0));
  }

  // TEMPORARY - USED FOR TESTING
  private static void exampleMethod() {
    Debug.Log("Space button pressed! Frame: " + Time.frameCount);
  }

  // TEMPORARY - USED FOR TESTING
  private static void exampleMouseMethod()
  {
    Debug.Log("Mouse button pressed! Frame: " + Time.frameCount);
  }
}
