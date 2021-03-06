﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class StartMenu : MonoBehaviour {

  public AudioClip track;

  private void Awake()
  {
    AudioManager manager = gameObject.GetComponent<AudioManager>();
    manager.PlayMusic(track, fadeDuration: 5);
  }

  private void Update()
  {
   bool A = Input.GetButtonDown("AButton");
    if (A)
    {
      PlayGame();
    }
    bool B = Input.GetButtonDown("BButton");
    if (B) {
      QuitGame();
    }
  }

  public void PlayGame()
    {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }
}
