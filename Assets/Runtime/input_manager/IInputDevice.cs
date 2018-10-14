using XInputWrapper;

public interface IInputDevice { }

public interface IButton : IInputDevice {
  bool InputActive();
}

public interface IAnalog : IInputDevice { }

public interface IAnalog1D : IAnalog {
  float Axis();
}

public interface IAnalog2D : IAnalog {
  Thumbstick Axes();
}