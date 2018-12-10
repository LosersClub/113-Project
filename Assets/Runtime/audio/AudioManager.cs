using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

  [HideInInspector]
  public bool isPlaying;

  public AudioClip menuTrack;
  public AudioClip templeTrack;
  public AudioClip farmlandTrack;
  public float fadeDuration;
  public float startDelay;
  public float maxVolume;
  public bool fade;

  private AudioSource audioSource;
  private bool fadingIn;
  private float fadeDur;
  private float maxVol;

  public void PlayMusic(AudioClip track, float delay=-1, float maxVolume=-1, float fadeDuration=-1) {
    if (gameObject.GetComponent<AudioSource>() == null)
    {
      this.audioSource = gameObject.AddComponent<AudioSource>();
    } else
    {
      this.audioSource = gameObject.GetComponent<AudioSource>();
    }
    this.audioSource.loop = true;
    this.fadeDuration = this.fadeDuration > 0 ? this.fadeDuration : 1;
    this.maxVol = maxVolume >= 0 ? maxVolume : this.maxVolume;
    this.fadeDur = fadeDuration >= 0 ? fadeDuration : this.fadeDuration;
    this.audioSource.clip = track;
    this.audioSource.PlayDelayed(delay >= 0 ? delay : this.startDelay);
    if (fade) {
      this.audioSource.volume = 0;
      this.fadingIn = true;
    } else {
      this.audioSource.volume = this.maxVol;
    }
  }

  public void StopMusic(bool stopImmediately = false, float fadeDuration = -1) {
    this.fadeDur = fadeDuration >= 0 ? fadeDuration : this.fadeDuration;
    if (!this.audioSource.isPlaying) {
      return;
    }
    if (fade) {
      if (stopImmediately)
      {
        this.audioSource.Stop();
      }
      this.StartCoroutine(this.FadeOutMusic());

    }
    else {
      this.audioSource.Stop();
    }
  }

  private void Update() {
    this.isPlaying = this.audioSource.isPlaying;
    if (this.audioSource.time > 0 && this.fadingIn && this.audioSource.volume < this.maxVol) {
      this.audioSource.volume += Time.deltaTime / this.fadeDur;
      if (this.audioSource.volume >= this.maxVol) {
        this.fadingIn = false;
      }
    }
  }

  private IEnumerator FadeOutMusic() {
    while (this.audioSource.volume > 0) {
      this.audioSource.volume -= Time.deltaTime / this.fadeDur;
      yield return null;
    }
    this.audioSource.Stop();
  }
}

