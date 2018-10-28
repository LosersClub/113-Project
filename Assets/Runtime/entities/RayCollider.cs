using System;
using UnityEngine;

[Serializable]
public class RayCollider {
  [SerializeField, Range(2, 20)]
  private int horizontalRays = 8;
  [SerializeField, Range(2, 20)]
  private int verticalRays = 4;
  [SerializeField, Range(0.001f, 0.3f)]
  private float rayInset = 0.02f;

  public Vector2 Spacing { get; private set; }
  public float Inset {
    get { return rayInset; }
    set {
      this.rayInset = value;
      this.RecalculateRaySpacing();
    }
  }
  public int HorizontalRays { get { return this.horizontalRays; } }
  public int VerticalRays { get { return this.verticalRays; } }

  public RaycastOrigins origins;

  private Vector2 bounds;
  public Vector2 Bounds {
    set {
      this.bounds = value;
      this.RecalculateRaySpacing();
    }
  }


  public void RecalculateRaySpacing() {
    this.Spacing = Rays.CalculateRaySpacing(this.bounds, this.rayInset,
      this.verticalRays, this.horizontalRays);
  }

  public void UpdateOrigins(BoxCollider2D boxCollider) {
    Rays.UpdateRayOrigins(boxCollider, this.Inset, ref this.origins);
  }
}