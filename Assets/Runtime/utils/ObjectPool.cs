using UnityEngine;
using System;

public class MonoObjectPool<I> : MonoBehaviour where I : PoolItem<I>, new() {
  public GameObject prefab;
  public int capacity;
  public bool fill = true;

  private ObjectPool<I> pool;

  private void Start() {
    this.pool = new ObjectPool<I>(this.prefab, this.capacity, this.fill);
    this.pool.OnCreate += SetParent;
  }

  private void SetParent(I item) {
    item.instance.transform.SetParent(transform);
  }
}

public class ObjectPool<I> where I : PoolItem<I>, new() {
  private const int DEFAULT_CAPACITY = 10;
  private readonly GameObject prefab;
  private readonly int capacity;
  public delegate void CreateEvent(I item);

  private int size = 0;
  private I last = null;
  private I inactive = null;

  public ObjectPool(GameObject prefab, bool fill = false) : this(prefab, DEFAULT_CAPACITY) { }
  public ObjectPool(GameObject prefab, int capacity, bool fill = false) {
    this.prefab = prefab;
    this.capacity = capacity;

    this.inactive = this.last = this.AddNode(this.CreateItem());
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
    item.pool = this;
    if (this.OnCreate != null) {
      this.OnCreate(item);
    }
    item.SetReferences();
    item.Sleep();
    return item;
  }

  public I Pop() {
    I item = null;
    if (this.inactive == null) {
      item = this.CreateItem();
    } else {
      item = this.inactive;
      this.inactive = (I)this.inactive.next;
    }
    item.WakeUp();
    return item;
  }

  public void Return(I item) {
    item.Sleep();
    this.AddNode(item).next = null;
    if (this.inactive == null) {
      this.inactive = this.last;
    }
  }
}

public abstract class PoolItem<I> where I : PoolItem<I>, new() {
  public I next;
  public ObjectPool<I> pool;
  public GameObject instance;

  public abstract void SetReferences();
  public abstract void Sleep();
  public abstract void WakeUp();

  public virtual void Return() {

  }
}