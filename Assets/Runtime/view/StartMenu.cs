using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class StartMenu : MonoBehaviour {
    public void PlayGame()
    {
        SceneManager.LoadScene("PlayerDemo");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); 

    }
}
