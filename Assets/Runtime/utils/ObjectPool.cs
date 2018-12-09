using UnityEngine;

public class ObjectPool<I> where I : PoolItem<I>, new() {
  private const int DEFAULT_CAPACITY = 10;
  private readonly GameObject prefab;
  private readonly Transform parent;
  private readonly int capacity;
  public delegate void CreateEvent(I item);

  private int size = 0;
  private I last = null;
  private I inactive = null;
  private I head = null;

  public ObjectPool(GameObject prefab, bool fill = false, Transform parent = null) : this(prefab, DEFAULT_CAPACITY, fill, parent) { }
  public ObjectPool(GameObject prefab, int capacity, bool fill = false, Transform parent = null) {
    this.prefab = prefab;
    this.capacity = capacity;
    this.parent = parent;

    this.head = this.inactive = this.last = this.CreateItem();
    this.size = 1;
    if (fill) {
      for (int i = 1; i < this.capacity; i++) {
        this.AddNode(this.CreateItem());
      }
    }
  }

  #region Properties
  public GameObject Prefab { get { return this.prefab; } }
  public int Size { get { return this.size; } }
  public int Capacity { get { return this.capacity; } }
  public event CreateEvent OnCreate;
  #endregion

  private I AddNode(I node) {
    node.prev = this.last;
    return this.last = this.last.next = node;
  }

  private I CreateItem() {
#if UNITY_EDITOR
    if (this.size >= this.capacity) {
      Debug.Log("ObjectPool is max capcity, Overflowing!");
    }
#endif
    size++;
    I item = new I();
    item.instance = GameObject.Instantiate(this.prefab);
    item.instance.transform.parent = this.parent;
    item.pool = this;
    if (this.OnCreate != null) {
      Debug.Log("Called");
      this.OnCreate(item);
    }
    item.SetReferences(this);
    item.Sleep();
    return item;
  }

  public I Pop() {
    I item = null;
    if (this.inactive == null) {
      item = this.AddNode(this.CreateItem());
    } else {
      item = this.inactive;
      this.inactive = (I)this.inactive.next;
    }
    item.WakeUp();
    return item;
  }

  public void Return(I item) {
    item.Sleep();
    if (item.prev != null) {
      item.prev.next = item.next;
    }
    if (item.next != null) {
      item.next.prev = item.prev;
    }
    if (item == head) {
      head = item.next;
    }
    item.prev = this.AddNode(item).next = null;
    if (this.inactive == null) {
      this.inactive = this.last;
    }
  }

  public void ReturnAll() {
    I next = null;
    for (I node = this.head; node != null && node != inactive; node = next) {
      next = node.next;
      Return(node);
    }
  }
}

public abstract class PoolItem<I> where I : PoolItem<I>, new() {
  public I prev, next;
  public ObjectPool<I> pool;
  public GameObject instance;

  public abstract void SetReferences(ObjectPool<I> pool);
  public abstract void Sleep();
  public abstract void WakeUp();

  public virtual void Return() {
    this.pool.Return((I)this);
  }
}

public class GenericItem : PoolItem<GenericItem> {
  public override void SetReferences(ObjectPool<GenericItem> pool) {
    this.instance.AddComponent<PoolTracker>().Set(this);
  }

  public override void Sleep() {
    this.instance.SetActive(false);
  }

  public override void WakeUp() {
    this.instance.SetActive(true);
  }
}

[System.Serializable]
public class GenericPool : MonoPool<GenericItem> { }

[System.Serializable]
public class MonoPool<T> where T : PoolItem<T>, new() {
  public GameObject prefab;
  public int capacity;

  private ObjectPool<T> pool;

  public void Init(GameObject parent) {
    this.pool = new ObjectPool<T>(this.prefab, this.capacity,
        parent: parent.transform, fill: true);
  }

  public void Return(T item) {
    this.pool.Return(item);
  }

  public T Pop() {
    return this.pool.Pop();
  }

  public T Pop(Vector3 location) {
    T item = this.pool.Pop();
    item.instance.transform.localPosition = location;
    return item;
  }

  public void ReturnAll() {
    this.pool.ReturnAll();
  }
}

public class PoolTracker : MonoBehaviour {
  private GenericItem instance;

  public PoolTracker Set(GenericItem instance) {
    this.instance = instance;
    return this;
  }

  public void Return() {
    this.instance.Return();
  }
}