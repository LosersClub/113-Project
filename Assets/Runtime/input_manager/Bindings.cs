using UnityEngine;
using XInputWrapper;

public class Bindings {
  public static void Initialize() {

    InputManager.AddButtonAction("Right", () => GameManager.Player.Move(1.0f, 0.0f),
      new KeyboardButton(KeyCode.D));
    InputManager.AddButtonAction("Left", () => GameManager.Player.Move(-1.0f, 0.0f),
      new KeyboardButton(KeyCode.A));
    InputManager.AddButtonAction("Crouch", () => GameManager.Player.Move(0.0f, -1.0f),
      new KeyboardButton(KeyCode.S));
    InputManager.AddButtonAction("Jump", GameManager.Player.Jump,
      new KeyboardButton(KeyCode.Space));

    InputManager.AddAnalog2DAction("Controller move", GameManager.Player.Move,
      new ControllerAnalog2D(Side.Left));
    InputManager.AddButtonAction("Controller Jump", GameManager.Player.Jump,
      new ControllerButton(Button.A));
  }
}