using UnityEngine;

[System.Serializable]
public struct Platform {
  public Vector2 start;
  public int size;

  public Platform(Vector2 start, int size) {
    this.start = start;
    this.size = size;
  }
}