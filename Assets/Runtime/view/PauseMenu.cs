using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

    public static bool GameIsPaused = false; 
	public GameObject pauseMenuPanel;

  private bool pauseHeld = false;
  private bool togglePause = true;

  public void PauseInput() {
    this.pauseHeld = true;
  }

	void Update () {
    if (this.togglePause && this.pauseHeld) {
      if (GameIsPaused) Resume();
      else Pause();
      this.togglePause = false;
    }
    if (!this.pauseHeld) {
      this.togglePause = true;
    }
    this.pauseHeld = false;
	}

    public void Resume()
    {
        GameManager.Player.EnableInput();
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1;
        GameIsPaused = false;
    }

    public void Pause()
    {
        GameManager.Player.DisableInput();
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0;
        GameIsPaused = true;
    }

    public void AdjustSound()
    {

    }

    public void LoadMenu()
    {
        
    }

    // quit game button?? 
}
