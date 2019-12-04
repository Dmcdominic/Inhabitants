using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fade_out : MonoBehaviour {

  // Public fields
  public float fade_duration = 10f;
  public bool fade_on_start = true;

  // Private vars
  private SpriteRenderer sr;
  private bool fading_out = false;
  private float t;


  // Init
  private void Awake() {
    sr = GetComponent<SpriteRenderer>();
  }

  // Start is called before the first frame update
  void Start() {
    if (fade_on_start) {
      fadeOut();
    }
  }

  // Update is called once per frame
  void Update() {
    if (fading_out) {
      t -= Time.deltaTime;
      if (t < 0) {
        fading_out = false;
        color_util.set_alpha(sr, 0);
        return;
      }

      float a = Mathf.Lerp(0f, 1f, t / fade_duration);
      color_util.set_alpha(sr, a);
    }
  }

  // Fade out
  private void fadeOut() {
    t = fade_duration;
    fading_out = true;
  }
}
