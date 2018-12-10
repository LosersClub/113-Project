using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class RoomGenerator {
  public Probabilities probs;

  [MinMax(28, 300)]
  public MinMax widthRange = new MinMax(28, 100);
  [MinMax(16, 100)]
  public MinMax heighRange = new MinMax(16, 50);

  [Header("Ground Settings")]
  [Range(0, 10)]
  public int maxBumps = 5;
  public int minGroundWidth = 5;
  public int maxGroundHeight = 4;
  public int groundHeight = 2;

  [Header("Island Settings")]
  public int islandClearance = 4;

  [Serializable]
  public class Probabilities {
    [Range(0f, 1f)]
    public float symmetric = 0.5f;
    [Range(0f, 1f)]
    public float groundBumps = 0.8f;
    [Range(0f, 1f)]
    public float groundHeight = 0.5f;
    [Range(0f, 1f)]
    public float groundWidth = 0.8f;
  }

  public Room Generate() {
    bool symmetric = Random.value <= this.probs.symmetric;
    Room room = new Room(this.widthRange.Random(), this.heighRange.Random(), symmetric);
    int effectiveWidth = symmetric ? room.Width / 2 : room.Width;

    this.GenerateGround(room, effectiveWidth);

    return room;
  }

  private void GenerateGround(Room room, int roomWidth) {
    room.AddGround(); // TODO: Pass in height

    int bumps = FalloffRandom(this.probs.groundBumps, this.maxBumps);
    for (int b = 0; b < bumps;) {
      int startX = Random.Range(this.minGroundWidth, room.Symmetric ? roomWidth - this.minGroundWidth :
          roomWidth - 2 * this.minGroundWidth + 1);
      int startY = Random.Range(this.groundHeight, this.maxGroundHeight);

      int height = FalloffRandom(this.probs.groundHeight, this.maxGroundHeight);
      if (startY - height < 0) {
        height = startY;
      }

      int width = FalloffRandom(this.probs.groundWidth) + this.minGroundWidth;
      if (startX + width > roomWidth) {
        width = roomWidth - startX;
      } else if (room.Symmetric && startX + width >= roomWidth - this.minGroundWidth) {
        startX = roomWidth - width;
      }

      Vector2Int start = new Vector2Int(startX, startY);
      Vector2Int end = new Vector2Int(startX + width - 1, startY - height + 1);
      if (this.BuildChunk(room, ref start, ref end, this.minGroundWidth)) {
        room.AddChunk(new Chunk(start, end));
        b++;
      }
    }
  }

  private bool BuildChunk(Room room, ref Vector2Int start, ref Vector2Int end, int fillDist) {
    foreach (Chunk chunk in room.Chunks) {
      // Check above clearance
      if (chunk.end.y > start.y && chunk.end.y - start.y <= this.islandClearance) {
        if ((chunk.end.x < start.x && start.x - chunk.end.x <= this.islandClearance) ||
            (chunk.start.x < end.x && chunk.start.x - end.x <= islandClearance) ||
            (chunk.end.x >= start.x && chunk.start.x >= end.x)) {
          return false;
        }
      }

      // Check below clearance
      if (end.y > chunk.end.y && end.y - chunk.start.y <= 1 &&
            ((chunk.start.x > start.x - this.islandClearance && chunk.start.x < end.x + this.islandClearance) ||
            (chunk.end.x < end.x  + this.islandClearance && chunk.end.x > start.x + this.islandClearance))) {
        return false;
      } else if (end.y > chunk.start.y && end.y - chunk.start.y <= this.islandClearance) {
        if ((chunk.end.x < start.x && start.x - chunk.end.x <= this.islandClearance) ||
            (chunk.start.x < end.x && chunk.start.x - end.x <= islandClearance) ||
            (chunk.end.x >= start.x && chunk.start.x >= end.x)) {
          return false;
        }
      }

      // Resolve gaps
      if (chunk.end.x < start.x && start.x - chunk.end.x <= fillDist) {
        start.x = chunk.end.x + 1;
      }
      if (chunk.start.x > end.x && chunk.start.x - end.x <= fillDist) {
        end.x = chunk.start.x - 1;
      }
    }
    // TODO: Graph building for feasibility here?
    return true;
  }


  public static int FalloffRandom(float prob, int max = int.MaxValue) {
    int value = 0;
    while (Random.value <= prob && value < max) {
      value++;
    }
    return value;
  }
}