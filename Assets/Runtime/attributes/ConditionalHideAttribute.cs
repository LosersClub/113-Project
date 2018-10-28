using UnityEngine;

public class ConditionalHideAttribute : PropertyAttribute {
  public string ConditionalSource = "";
  public bool HideInInsepctor = true;
  public bool Inverse = false;

  public ConditionalHideAttribute(string conditionalSource, bool hide = true, bool inverse = false) {
    this.ConditionalSource = conditionalSource;
    this.HideInInsepctor = hide;
    this.Inverse = inverse;
  }
}