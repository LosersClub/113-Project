using UnityEngine;
using System;

[System.Serializable]
public struct Chunk {
  public Vector2Int start;
  public Vector2Int end;

  public Chunk(Vector2Int start, Vector2Int end) {
    this.start = start;
    this.end = end;
  }

  public override bool Equals(object obj) {
    return obj is Chunk && ((Chunk)obj).start == this.start && ((Chunk)obj).end == this.end;
  }

  public override int GetHashCode() {
    return start.GetHashCode() ^ end.GetHashCode();
  }
}