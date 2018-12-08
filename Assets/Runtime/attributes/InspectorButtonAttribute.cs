using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field)]
public class InspectorButtonAttribute : PropertyAttribute {
  public static float DefaultButtonWidth = 80;

  public readonly string MethodName;

  private float buttonWidth = DefaultButtonWidth;
  public float ButtonWidth {
    get { return this.buttonWidth; }
    set { buttonWidth = value; }
  }

  public InspectorButtonAttribute(string MethodName) {
    this.MethodName = MethodName;
  }
}