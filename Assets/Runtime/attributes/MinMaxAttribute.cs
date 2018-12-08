using UnityEngine;

public class MinMaxAttribute : PropertyAttribute {
  public int MinLimit = 0;
  public int MaxLimit = 10;

  public MinMaxAttribute(int minLimit, int maxLimit) {
    this.MinLimit = minLimit;
    this.MaxLimit = maxLimit;
  }
}