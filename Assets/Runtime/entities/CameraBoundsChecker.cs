using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Checks if the GameObject is out of bounds of the main Camera, using the GameObject's Collider2D
/// component.
/// </summary>
/// <remarks>
/// Methods optionally take a buffer argument. The buffer indicates how far past the exact edge of
/// the camera bound the GameObject must be in order to be considered past that bound.
/// </remarks>

[RequireComponent(typeof(Collider2D))]
public class CameraBoundsChecker : MonoBehaviour {

  private Collider2D collider2DComponent;

  private Camera boundsCamera;

  void Start () {
    this.collider2DComponent = this.GetComponent<Collider2D>();

    this.boundsCamera = Camera.main;
    Assert.IsNotNull(this.boundsCamera);
    Assert.IsTrue(this.boundsCamera.orthographic);
  }

  public bool IsPastLeft(float buffer=0) {
    return this.collider2DComponent.bounds.max.x < this.CameraLowerLeft.x - buffer;
  }

  public bool IsPastRight(float buffer=0) {
    return this.collider2DComponent.bounds.min.x > this.CameraUpperRight.x + buffer;
  }

  public bool IsPastBottom(float buffer=0) {
    return this.collider2DComponent.bounds.max.y < this.CameraLowerLeft.y - buffer;
  }

  public bool IsPastTop(float buffer=0) {
    return this.collider2DComponent.bounds.min.y > this.CameraUpperRight.y + buffer;
  }

  public bool IsOutOfBounds(float buffer=0) {
    return (this.IsPastLeft(buffer) || this.IsPastRight(buffer)
            || this.IsPastBottom(buffer) || this.IsPastTop(buffer));
  }

  private Vector2 CameraLowerLeft {
    get {
      return new Vector2(this.boundsCamera.transform.position.x - this.boundsCamera.orthographicSize * this.boundsCamera.aspect,
                          this.boundsCamera.transform.position.y - this.boundsCamera.orthographicSize);
    }
  }

  private Vector2 CameraUpperRight {
    get {
      return new Vector2(this.boundsCamera.transform.position.x + this.boundsCamera.orthographicSize * this.boundsCamera.aspect,
                          this.boundsCamera.transform.position.y + this.boundsCamera.orthographicSize);
    }
  }
}
