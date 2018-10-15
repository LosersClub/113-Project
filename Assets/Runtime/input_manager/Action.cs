interface IAction {
  void Update();
  int Rebind(IInputDevice binding);
}

interface IAnalogAction : IAction { }