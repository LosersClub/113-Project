using UnityEngine;

public class SimpleLevel : Level {
  [Header("Generation Settings")]
  public int roomCount = 3;
  public RoomGenerator generator;


  public bool manualLastRoom = false;
  [ConditionalHide("manualLastRoom")]
  public ManualRoom lastRoom;
  public CustomRoom[] customRooms;

  [Header("Prefabs")]
  public GenericPool ground;
  public GenericPool groundFiller;
  public GenericPool leftPlatform;
  public GenericPool mainPlatform;
  public GenericPool rightPlaform;

  public override Room[] GenerateRooms() {
    Room[] rooms = new Room[this.roomCount];
    this.ground.Init(this.gameObject);
    this.groundFiller.Init(this.gameObject);
    this.leftPlatform.Init(this.gameObject);
    this.mainPlatform.Init(this.gameObject);
    this.rightPlaform.Init(this.gameObject);

    for (int i = 0; i < roomCount; i++) {
      if (manualLastRoom && i == roomCount - 1) {
        rooms[i] = lastRoom.Get();
        continue;
      }

      bool custom = false;
      foreach (CustomRoom cr in this.customRooms) {
        if (cr.index == i) {
          rooms[i] = cr.room.Get();
          custom = true;
          break;
        }
      }
      if (custom) {
        continue;
      }

      //rooms[i] = generator.Generate();
      rooms[i] = new Room(28, 16); // TODO: pick from Ranges EnemySpawner.RandomFromRange

      rooms[i].AddGround();
      rooms[i].AddPlatform(new Platform(new Vector2(4.5f, 4.5f), 6));
    }

    return rooms;
  }

  public override void RenderRoom(Room room) {
    for (int i = 1; i <= 2; i++) {
      this.ground.Pop(new Vector3(-i, 1));
      this.groundFiller.Pop(new Vector3(-i, 0));
      this.ground.Pop(new Vector3(room.Width - 1 + i, 1));
      this.groundFiller.Pop(new Vector3(room.Width - 1 + i, 0));
    }

    foreach (Tile tile in room.Tiles) {
      GenericPool pool = room.TileAt(tile.x, tile.y + 1) ? this.groundFiller : this.ground;
      pool.Pop(new Vector3(tile.x, tile.y));

      if (room.Symmetric) {
        pool.Pop(new Vector3(room.Width - tile.x - 1, tile.y));
      }
    }

    foreach (Platform platform in room.Platforms) {
      Vector3 position = new Vector3(platform.start.x - 0.25f, platform.start.y - 0.25f);
      this.leftPlatform.Pop(position);
      position.x += 0.5f;
      for (int x = 0; x < platform.size; x++, position.x += 0.5f) {
        this.mainPlatform.Pop(position);
      }
      this.rightPlaform.Pop(position);

      // TODO: Symmetric
    }

    if (room is MonoRoom) {
      ((MonoRoom)room).CustomRender();
    }
  }

  public override void UnloadRoom(Room room) {
    if (room is MonoRoom) {
      ((MonoRoom)room).CustomUnload();
    }
    this.ground.ReturnAll();
    this.groundFiller.ReturnAll();
    this.leftPlatform.ReturnAll();
    this.mainPlatform.ReturnAll();
    this.rightPlaform.ReturnAll();
  }

  [System.Serializable]
  public struct CustomRoom {
    public int index;
    public ManualRoom room;
  }
}