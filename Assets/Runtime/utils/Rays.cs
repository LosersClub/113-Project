using UnityEngine;

public struct RaycastOrigins {
  public Vector2 topLeft;
  public Vector2 bottomLeft;
  public Vector2 bottomRight;
}

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

  public static void UpdateRayOrigins(BoxCollider2D collider, float inset, ref RaycastOrigins origins) {
    Bounds insetBounds = collider.bounds;
    insetBounds.Expand(-2f * inset);

    origins.topLeft = new Vector2(insetBounds.min.x, insetBounds.max.y);
    origins.bottomLeft = insetBounds.min;
    origins.bottomRight = new Vector2(insetBounds.max.x, insetBounds.min.y);
  }

  public static Vector2 CalculateRaySpacing(Vector2 bounds, float inset,
      int verticalRays = 2, int horizontalRays = 2) {
    return new Vector2(
      (bounds.x - (2f * inset)) / (verticalRays - 1),
      (bounds.y - (2f * inset)) / (horizontalRays - 1)
    );
  }
}