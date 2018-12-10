using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Storyboard : MonoBehaviour {
  public float duration = 5f;
  public Image[] panels;
  public AudioClip track;


  public void OnEnable() {
    AudioManager manager = gameObject.GetComponent<AudioManager>();
    manager.PlayMusic(track);
    this.StartCoroutine(this.NextPanel());
  }

  private IEnumerator NextPanel() {
    for (int i = 0; i < panels.Length; i++) {
      this.panels[i].enabled = true;
      float timer = this.duration;
      while (timer > 0) {
        timer -= Time.deltaTime;
        yield return null;
      }
      panels[i].enabled = false;
    }
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
  }
}