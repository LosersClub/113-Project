using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour {

  public float assetSize;
  public int numLayers;
  public float[] parallaxX;
  public float[] parallaxY;

  private Transform cameraTransform;
  private Transform[][] layers;
  private Transform[] fullLayers;
  private float viewZone = 10;
  private int numTiles = 3;
  private int leftIndex;
  private int rightIndex;
  private float lastCameraX;
  private float lastCameraY;

  private void Start() {
    cameraTransform = Camera.main.transform;
    layers = new Transform[transform.childCount][];
    fullLayers = new Transform[transform.childCount];
    for (int i = 0; i < transform.childCount; i++) {
      fullLayers[i] = transform.GetChild(i);
      layers[i] = new Transform[fullLayers[i].childCount];
      for (int j = 0; j < layers[i].Length; j++) {
        layers[i][j] = fullLayers[i].GetChild(j);
      }
    }
    leftIndex = 0;
    rightIndex = this.numTiles - 1;
    lastCameraX = cameraTransform.position.x;
    lastCameraY = cameraTransform.position.y;
  }

  private void Update() {
    float deltaX = cameraTransform.position.x - lastCameraX;
    float deltaY = cameraTransform.position.y - lastCameraY;
    for (int i = 0; i < this.layers.Length; i++) {
      fullLayers[i].position +=
        new Vector3(deltaX * this.parallaxX[i], deltaY * this.parallaxY[i], 0);
      if (cameraTransform.position.x < layers[i][leftIndex].transform.position.x + viewZone)
      {
        ScrollLeft();
      }
      else if (cameraTransform.position.x > layers[i][rightIndex].transform.position.x - viewZone)
      {
        ScrollRight();
      }
    }
    lastCameraX = cameraTransform.position.x;
    lastCameraY = cameraTransform.position.y;
  }

  private void ScrollLeft() {
    for (int i = 0; i < this.layers.Length; i++)
    {
      layers[i][rightIndex].position =
       new Vector3((layers[i][leftIndex].position.x - assetSize),
       layers[i][rightIndex].position.y,
      layers[i][rightIndex].position.z);


    }
     leftIndex = rightIndex;
     rightIndex = (rightIndex - 1 < 0) ? layers.Length - 1 : rightIndex - 1;

  }

  private void ScrollRight() {
    for (int i = 0; i < this.layers.Length; i++)
    {
      layers[i][leftIndex].position =
      new Vector3((layers[i][rightIndex].position.x + assetSize),
        layers[i][rightIndex].position.y,
        layers[i][rightIndex].position.z);
    }
    rightIndex = leftIndex;
    leftIndex = (leftIndex + 1 == layers.Length) ? 0 : leftIndex + 1;
  }

}
