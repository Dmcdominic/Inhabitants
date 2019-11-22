using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pulse_alpha : MonoBehaviour {

  private SpriteRenderer sr;


  // Start is called before the first frame update
  void Start() {
    sr = GetComponent<SpriteRenderer>();
  }

  // Update is called once per frame
  void Update() {
    float t = 2f * Mathf.Abs(0.5f - (Time.time % 1f));
    color_util.set_alpha(sr, Mathf.Lerp(0.3f, 1f, t));
  }
}
