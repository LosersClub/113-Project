using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour {

  public float assetSize;
  public float parallaxSpeed;

  private Transform cameraTransform;
  private Transform[] layers;
  private float viewZone = 10;
  private int leftIndex;
  private int rightIndex;
  private float lastCameraX;

  private void Start() {
    cameraTransform = Camera.main.transform;
    layers = new Transform[transform.childCount];
    for (int i = 0; i < transform.childCount; i++) {
      layers[i] = transform.GetChild(i);
    }
    leftIndex = 0;
    rightIndex = layers.Length - 1;
    lastCameraX = cameraTransform.position.x;
  }

  private void Update() {
    float deltaX = cameraTransform.position.x - lastCameraX;
    transform.position += Vector3.right * (deltaX * parallaxSpeed);
    if (cameraTransform.position.x < layers[leftIndex].transform.position.x + viewZone) {
      ScrollLeft();
    } else if (cameraTransform.position.x > layers[rightIndex].transform.position.x - viewZone) {
      ScrollRight();
    }
    lastCameraX = cameraTransform.position.x;
  }

  private void ScrollLeft() {
    layers[rightIndex].position = 
      new Vector3((layers[leftIndex].position.x - assetSize),
        layers[rightIndex].position.y,
        layers[rightIndex].position.z);
    leftIndex = rightIndex;
    rightIndex = (rightIndex - 1 < 0) ? layers.Length - 1 : rightIndex - 1;

  }

  private void ScrollRight() {
    int lastLeft = leftIndex;
    layers[leftIndex].position = 
      new Vector3((layers[rightIndex].position.x + assetSize), 
        layers[rightIndex].position.y, 
        layers[rightIndex].position.z);
    rightIndex = leftIndex;
    leftIndex = (leftIndex + 1 == layers.Length) ? 0 : leftIndex + 1;
  }

}
