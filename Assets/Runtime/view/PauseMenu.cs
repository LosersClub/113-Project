using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

    public static bool GameIsPaused = false; 
	public GameObject pauseMenuPanel;

	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused) Resume();
            else Pause(); 
        }
	}

    public void Resume()
    {
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1;
        GameIsPaused = false; 
    }

    public void Pause()
    {
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
