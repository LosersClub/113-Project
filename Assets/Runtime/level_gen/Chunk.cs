using UnityEngine;

public struct Chunk {
  public Vector2Int start;
  public Vector2Int end;

  public Chunk(Vector2Int start, Vector2Int end) {
    this.start = start;
    this.end = end;
  }
}