using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {
  public Level[] levels;
  public GameObject trinityPrefab;
  public GameObject damagerPrefab;

  public GameObject LeftWall { get; private set; }
  public GameObject RightWall { get; private set; }

  public GameObject LeftTrinity { get; private set; }
  public GameObject RightTrinity { get; private set; }

  private GameObject leftDamager;
  private GameObject rightDamager;

  public Level Active { get; private set; }

  private GameObject levelManager;
  private int activeLevel = 0;

  private void Awake() {
    this.levelManager = new GameObject("Level Manager");
    this.levelManager.transform.position = Vector3.zero;

    this.LeftWall = new GameObject("Left Wall");
    this.LeftWall.AddComponent<BoxCollider2D>();
    this.LeftWall.transform.SetParent(this.levelManager.transform);
    this.LeftWall.layer = LayerMask.NameToLayer("Impassable");
    this.LeftWall.SetActive(false);

    this.RightWall = new GameObject("Right Wall");
    this.RightWall.AddComponent<BoxCollider2D>();
    this.RightWall.transform.SetParent(this.levelManager.transform);
    this.RightWall.layer = LayerMask.NameToLayer("Impassable");
    this.RightWall.SetActive(false);

    this.LeftTrinity = Instantiate(trinityPrefab);
    this.LeftTrinity.transform.SetParent(this.levelManager.transform);
    this.LeftTrinity.SetActive(false);

    this.RightTrinity = Instantiate(trinityPrefab);
    this.RightTrinity.transform.SetParent(this.levelManager.transform);
    this.RightTrinity.SetActive(false);

    this.leftDamager = Instantiate(damagerPrefab);
    this.leftDamager.transform.SetParent(this.levelManager.transform);
    this.leftDamager.SetActive(false);

    this.rightDamager = Instantiate(damagerPrefab);
    this.rightDamager.transform.SetParent(this.levelManager.transform);
    this.rightDamager.SetActive(false);
  }

  // TODO: Delete
  private void OnEnable() {
    this.StartFirstLevel();
  }

  public void StartFirstLevel() {
    this.activeLevel = 0;
    this.StartLevel();
  }

  public void NextLevel() {
    this.activeLevel++;
    if (this.activeLevel >= this.levels.Length) {
      // TODO, no more levels idk what to do?
      return;
    }
    this.Active.background.SetActive(false);
    Destroy(this.Active.gameObject);
    this.StartLevel();
  }

  public void RestartLevel() {
    this.LeftTrinity.SetActive(false);
    this.RightTrinity.SetActive(false);
    this.Active.background.SetActive(false);
    Destroy(this.Active.gameObject);
    this.StartLevel();
  }

  public void StartLevel() {
    this.Active = Instantiate<Level>(this.levels[this.activeLevel]);
    this.Active.transform.SetParent(this.levelManager.transform);
    this.Active.transform.localPosition = Vector3.zero;
    this.Active.gameObject.SetActive(true);
    this.Active.background.SetActive(true);
    this.Active.Enable();
    GameManager.AudioManager.PlayMusic(this.Active.track, fadeDuration: 5);
  }

  public void SetWalls(int width, int maxHeight) {
    this.LeftWall.transform.localScale = new Vector3(1, maxHeight, 1);
    this.RightWall.transform.localScale = new Vector3(1, maxHeight, 1);

    this.LeftWall.transform.localPosition = new Vector3(-1, maxHeight / 2);
    this.RightWall.transform.localPosition = new Vector3(width, maxHeight / 2);
  }

  public void StartBlockers(int width, int height) {
    this.LeftTrinity.transform.localPosition = new Vector3(0, height - 1f);
    this.LeftTrinity.SetActive(true);
    this.RightTrinity.transform.localPosition = new Vector3(width - 1, height - 1f);
    this.RightTrinity.SetActive(true);

    this.LeftTrinity.GetComponent<SpriteRenderer>().enabled = true;
    this.RightTrinity.GetComponent<SpriteRenderer>().enabled = true;
    this.LeftTrinity.GetComponentInChildren<ParticleSystem>().Play();
    this.RightTrinity.GetComponentInChildren<ParticleSystem>().Play();
  }

  public void StartDamagers(int width, int height) {
    this.leftDamager.transform.localScale = new Vector3(1, height, 1);
    this.rightDamager.transform.localScale = new Vector3(1, height, 1);

    this.leftDamager.transform.localPosition = new Vector3(0, height / 2);
    this.leftDamager.SetActive(true);
    this.rightDamager.transform.localPosition = new Vector3(width - 1f, height / 2);
    this.rightDamager.SetActive(true);
  }

  public void StopBlockers() {
    this.LeftTrinity.GetComponentInChildren<ParticleSystem>().Stop();
    this.RightTrinity.GetComponentInChildren<ParticleSystem>().Stop();
    this.leftDamager.SetActive(false);
    this.rightDamager.SetActive(false);
    this.StartCoroutine(this.DeactivateTrinity());
  }

  private IEnumerator DeactivateTrinity() {
    yield return new WaitForSeconds(1f);
    this.LeftTrinity.GetComponent<SpriteRenderer>().enabled = false;
    this.RightTrinity.GetComponent<SpriteRenderer>().enabled = false;
  }
}