using System;

[Serializable]
public struct MinMax {
  public int min;
  public int max;

  public MinMax(int min, int max) {
    this.min = min;
    this.max = max;
  }

  public int Random() {
    return UnityEngine.Random.Range(this.min, this.max + 1);
  }
}