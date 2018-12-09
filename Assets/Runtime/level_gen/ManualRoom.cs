using UnityEngine;
using UnityEngine.Events;
using System;

public class ManualRoom : MonoBehaviour {
  public Vector2Int size = new Vector2Int(28, 16);
  public bool spawnEnemies = false;
  public bool useBlockers = true;
  public bool baseGround = true;

  public Chunk[] chunks;
  public Platform[] platforms;
  public Events events;

  [Serializable]
  public class Events {
    public RoomEvent generate;
    public RoomEvent render;
    public RoomEvent startRoom;
    public RoomEvent unload;
  }

  public Room Get() {
    Room room = new MonoRoom(this, size.x, size.y);
    if (this.baseGround) {
      room.AddGround();
    }

    foreach (Chunk c in this.chunks) {
      room.AddChunk(c);
    }
    foreach (Platform p in this.platforms) {
      room.AddPlatform(p);
    }
    this.events.generate.Invoke(room);
    return room;
  }

  [Serializable]
  public class RoomEvent : UnityEvent<Room> { }
}

public class MonoRoom : Room {
  private readonly ManualRoom handler;

  public MonoRoom(ManualRoom handler, int width, int height, bool symmetric = false)
      : base(width, height, symmetric) {
    this.handler = handler;
  }

  public bool UseSpawner {
    get {
      return this.handler.spawnEnemies;
    }
  }
  public bool UseBlockers {
    get {
      return this.handler.useBlockers;
    }
  }

  public void CustomRender() {
    this.handler.events.render.Invoke(this);
  }

  public void CustomUnload() {
    this.handler.events.unload.Invoke(this);
  }

  public void StartRoom() {
    this.handler.events.startRoom.Invoke(this);
  }
}