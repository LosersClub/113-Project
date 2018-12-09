using System.Collections.Generic;
using UnityEngine;

public class Room {
  private readonly int width;
  private readonly int height;
  private readonly List<Chunk> chunks;
  private readonly List<Platform> platforms;
  private readonly Dictionary<int, Tile> tiles;

  public Room(int width, int height, bool symmetric = false) {
    this.width = width + width % 2;
    this.height = height;
    this.Symmetric = symmetric;

    this.chunks = new List<Chunk>();
    this.platforms = new List<Platform>();
    this.tiles = new Dictionary<int, Tile>();
  }

  private int Hash(int x, int y) {
    return y * width + x;
  }

  public bool TileAt(int x, int y) {
    return this.tiles.ContainsKey(Hash(x, y));
  }

  public Chunk AddGround() {
    return this.AddChunk(new Chunk(new Vector2Int(0, 1), new Vector2Int(this.width - 1, 0)));
  }

  public Chunk AddChunk(Chunk chunk) {
    for (int x = chunk.start.x; x <= chunk.end.x; x++) {
      for (int y = chunk.end.y; y <= chunk.start.y; y++) {
        this.tiles[Hash(x, y)] = new Tile(x, y);
      }
    }
    this.chunks.Add(chunk);
    return chunk;
  }

  public Platform AddPlatform(Platform platform) {
    this.platforms.Add(platform);
    return platform;
  }

  public bool Symmetric { get; private set; }
  public int Width { get { return this.width; } }
  public int Height { get { return this.height; } }
  public ICollection<Tile> Tiles { get { return this.tiles.Values; } }
  public List<Chunk> Chunks { get { return this.chunks; } }
  public List<Platform> Platforms { get { return this.platforms; } }
}

public struct Tile {
  public int x;
  public int y;

  public Tile(int x, int y) {
    this.x = x;
    this.y = y;
  }
}