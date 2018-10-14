using UnityEngine;

public class Bindings {
  public static void Initialize() {
    InputManager.AddAction("mouse test", Bindings.exampleMouseMethod, new MouseButton(0));

    InputManager.AddAction("Right", () => GameManager.Player.Move(1.0f),
      new KeyboardButton(KeyCode.D));
    InputManager.AddAction("Left", () => GameManager.Player.Move(-1.0f),
      new KeyboardButton(KeyCode.A));
    InputManager.AddAction("Jump", GameManager.Player.Jump,
      new KeyboardButton(KeyCode.Space));
  }

  // TEMPORARY - USED FOR TESTING
  private static void exampleMouseMethod() {
    Debug.Log("Mouse button pressed! Frame: " + Time.frameCount);
  }
}