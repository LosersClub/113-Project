using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Storyboard : MonoBehaviour {
  public float[] durations = { 5f, 5f, 5f, 5f, 5f };
  public Image[] panels;
  public AudioClip track;


  public void OnEnable() {
    AudioManager manager = gameObject.GetComponent<AudioManager>();
    manager.PlayMusic(track);
    this.StartCoroutine(this.NextPanel());
  }

  private IEnumerator NextPanel() {
    int i;
    float timer;
    for (i = 0; i < panels.Length; i++) {
      this.panels[i].enabled = true;
      timer = this.durations[i];
      while (timer > 0) {
        timer -= Time.deltaTime;
        yield return null;
      }
      panels[i].enabled = false;
    }
    timer = this.durations[this.durations.Length - 1];
    while (timer > 0)
    {
      timer -= Time.deltaTime;
      yield return null;
    }
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
  }
}