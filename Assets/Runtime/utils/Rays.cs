using UnityEngine;

public static class Rays {
  public const float Precision = 0.001f;
  // NOT THREAD SAFE
  public static readonly RaycastHit2D[] singleHit = new RaycastHit2D[1];

  [System.Diagnostics.Conditional("DEBUG_PHYS_RAYS")]
  public static void DrawRay(Vector2 start, Vector2 dir, Color color) {
    Debug.DrawRay(start, dir, color);
  }

  [System.Diagnostics.Conditional("DEBUG_PHYS_RAYS")]
  public static void DrawRay(Vector2 start, Vector2 direction, float distance, Color color) {
    Debug.DrawRay(start, direction * distance, color);
  }
}