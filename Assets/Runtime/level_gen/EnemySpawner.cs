using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[Serializable]
public class EnemySpawner {
  [MinMax(1, 100)]
  public MinMax difficultyRange = new MinMax(5, 10);
  [MinMax(1, 100)]
  public MinMax activeDifficultyRange = new MinMax(3, 5);

  public Enemy[] enemies;

  public float difficultyCheckRate = 2f;
  public int portalCapacity = 10;
  public GameObject silhouettePrefab;
  public GameObject portalPrefab;

  int enemyCount = 0;
  int roomDifficulty = 0;
  private Level level;
  private readonly GenericPool spawnPortals = new GenericPool();
  private readonly GenericPool silhouettes = new GenericPool();

  public void Start(Level level) {
    this.level = level;
    this.spawnPortals.capacity = this.portalCapacity;
    this.spawnPortals.prefab = this.portalPrefab;
    this.silhouettes.prefab = this.silhouettePrefab;
    this.silhouettes.capacity = this.portalCapacity;
    this.spawnPortals.Init(level.gameObject);
    this.silhouettes.Init(level.gameObject);

    foreach (Enemy e in this.enemies) {
      e.pool.Init(level.gameObject);
    }
  }
  
  public void StartRoom(Room room) {
    this.enemyCount = 0;
    this.roomDifficulty = 0;
    int difficulty = this.difficultyRange.Random();
    Queue<EnemySpawn> spawns = new Queue<EnemySpawn>();

    room.Chunks.Sort((a, b) => a.start.y - b.start.y);

    while (difficulty > 0) {
      Enemy e = enemies[Random.Range(0, this.enemies.Length)];
      Vector2 location = Vector2.zero;
      if (Random.value <= 0.85f) {
        Chunk spawnChunk = room.Chunks[Random.Range(0, room.Chunks.Count)];
        location = new Vector2(Random.Range(spawnChunk.start.x, spawnChunk.end.x + 1),
          spawnChunk.start.y + Mathf.Abs(e.pool.prefab.GetComponent<SpriteRenderer>().bounds.extents.y) + 0.5f);
        foreach (Chunk c in room.Chunks) {
          if (c.Equals(spawnChunk)) {
            continue;
          }

          if (c.end.y <= spawnChunk.start.y + 2 && c.start.y >= spawnChunk.start.y) {
            if (location.x >= c.start.x && location.x <= c.end.x) {
              location.y = c.start.y + location.y - spawnChunk.start.y;
              spawnChunk = c;
            }
          }
        }

      } else if (room.Platforms.Count > 0) {
        Platform spawnPlatform = room.Platforms[Random.Range(0, room.Platforms.Count)];
        location = new Vector2(Random.Range(spawnPlatform.start.x, spawnPlatform.size + 1),
            spawnPlatform.start.y + e.pool.prefab.GetComponent<SpriteRenderer>().bounds.extents.y + 0.5f);
      }
      difficulty -= e.difficulty;
      spawns.Enqueue(new EnemySpawn(e, location));
    }
    this.level.StartCoroutine(this.CheckDifficulty(spawns, this.activeDifficultyRange.Random()));
  }

  public void SpawnEnemy(EnemySpawn spawn) {
    GenericItem portal = this.spawnPortals.Pop(spawn.location);
    Vector3 extents = spawn.enemy.pool.prefab.GetComponent<Renderer>().bounds.extents;
    portal.instance.transform.localScale = new Vector3(extents.x + 0.15f, extents.y, 1);
    level.StartCoroutine(this.SpawnEnemyRoutine(portal, spawn.enemy, spawn.location));
  }

  private IEnumerator CheckDifficulty(Queue<EnemySpawn> spawns, int target) {
    this.SpawnEnemies(spawns, target);
    float timer = this.difficultyCheckRate;
    while (spawns.Count > 0) {
      timer -= Time.deltaTime;
      if (enemyCount <= 0) {
        this.SpawnEnemies(spawns, target);
      } else if (timer < 0) {
        timer = this.difficultyCheckRate;
        this.SpawnEnemies(spawns, target);
      }
      yield return null;
    }

    while (enemyCount > 0) {
      yield return null;
    }
    this.level.StartCoroutine(this.level.ExitRoom());
  }

  private void SpawnEnemies(Queue<EnemySpawn> spawns, int target) {
    if (target <= 0) {
      return;
    }
    while (spawns.Count > 0 && roomDifficulty < target) {
      EnemySpawn spawn = spawns.Dequeue();
      this.roomDifficulty += spawn.enemy.difficulty;
      this.enemyCount++;
      this.SpawnEnemy(spawn);
    }
  }

  private IEnumerator SpawnEnemyRoutine(GenericItem portal, Enemy enemy, Vector2 location) {
    yield return new WaitForSeconds(0.875f);
    GenericItem silhouette = this.silhouettes.Pop(location); // TODO: set same scale as enemy?
    silhouette.instance.GetComponent<SpriteRenderer>().sprite = enemy.silhouette;

    yield return new WaitForSeconds(0.5f);
    portal.Return();
    silhouette.Return();
    EnemyPoolItem item = enemy.pool.Pop(location);
    DamageTaker taker = item.instance.GetComponent<DamageTaker>();
    item.instance.GetComponent<EnemyTracker>().difficulty = enemy.difficulty;
    item.instance.GetComponent<EnemyTracker>().parent = this;
    taker.OnDie.AddListener(this.OnEnemyDie);
  }

  public void OnEnemyDie(DamageDealer dealer, DamageTaker taker) {
    // TODO: Play death before return
    EnemyTracker tracker = taker.GetComponent<EnemyTracker>();
    this.roomDifficulty -= tracker.difficulty;
    this.enemyCount--;
    GenericItem death = this.silhouettes.Pop(tracker.transform.position);
    death.instance.GetComponent<SpriteRenderer>().sprite = tracker.GetComponent<SpriteRenderer>().sprite;
    this.level.StartCoroutine(DoDeath(death));
    tracker.Return();
  }

  private IEnumerator DoDeath(GenericItem death) {
    yield return this.level.StartCoroutine(death.instance.GetComponent<Dissolve>().DoDissolve(3f));
    death.Return();
    death.instance.GetComponent<Dissolve>().Set(0f);
  }

  public struct EnemySpawn {
    public Enemy enemy;
    public Vector2 location;

    public EnemySpawn(Enemy enemy, Vector2 location) {
      this.enemy = enemy;
      this.location = location;
    }
  }

  [Serializable]
  public class Enemy {
    public EnemyPool pool;
    public Sprite silhouette;
    [Range(1, 10)]
    public int difficulty;
  }

  [Serializable]
  public class EnemyPool : MonoPool<EnemyPoolItem> { }

  public class EnemyPoolItem : PoolItem<EnemyPoolItem> {
    public override void SetReferences(ObjectPool<EnemyPoolItem> pool) {
      this.instance.AddComponent<EnemyTracker>().Set(this);
    }

    public override void Sleep() {
      this.instance.SetActive(false);
    }

    public override void WakeUp() {
      this.instance.SetActive(true);
    }
  }

  public class EnemyTracker : MonoBehaviour {
    private EnemyPoolItem item;
    public int difficulty;
    public EnemySpawner parent;

    public EnemyTracker Set(EnemyPoolItem item) {
      this.item = item;
      return this;
    }

    private void Update() {
      if (this.transform.position.y < 0) {
        this.parent.OnEnemyDie(null, this.GetComponent<DamageTaker>());
      }
    }

    public void Return() {
      item.Return();
    }
  }
}