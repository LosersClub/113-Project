using UnityEngine;
using XInputWrapper;

public class Bindings {
  public static void Initialize() {

    InputManager.AddButtonAction("Right", () => GameManager.Player.Move(1.0f, 0.0f),
      new KeyboardButton(KeyCode.D));
    InputManager.AddButtonAction("Left", () => GameManager.Player.Move(-1.0f, 0.0f),
      new KeyboardButton(KeyCode.A));
    InputManager.AddButtonAction("Down", () => GameManager.Player.Move(0.0f, -1.0f),
      new KeyboardButton(KeyCode.S));
    InputManager.AddButtonAction("Up", () => GameManager.Player.Move(0.0f, 1.0f),
      new KeyboardButton(KeyCode.W));
    InputManager.AddButtonAction("Jump", GameManager.Player.Jump,
      new KeyboardButton(KeyCode.Space));
    InputManager.AddButtonAction("Dash", GameManager.Player.Dash,
      new KeyboardButton(KeyCode.LeftShift));
   
    //InputManager.AddButtonAction("Shoot", GameManager.Player.Shoot,
    //  new KeyboardButton(KeyCode.E));
    InputManager.AddButtonAction("Melee", GameManager.Player.Melee,
      new KeyboardButton(KeyCode.Q));

    InputManager.AddAnalog2DAction("Controller Move", GameManager.Player.Move,
      new ControllerAnalog2D(Side.Left));
    InputManager.AddButtonAction("Controller Jump", GameManager.Player.Jump,
      new ControllerButton(Button.A));
    InputManager.AddButtonAction("Controller Dash", GameManager.Player.Dash,
      new ControllerButton(Button.B));
    InputManager.AddButtonAction("Controller Melee", GameManager.Player.Melee,
      new ControllerButton(Button.X));
  }
}