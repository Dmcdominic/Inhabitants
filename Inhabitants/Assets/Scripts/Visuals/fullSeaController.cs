using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fullSeaController : MonoBehaviour {

  // Public fields
  public Sprite[] frames;

  // Components
  private SpriteRenderer sr;

  // Privat vars
  private float next_frame_time;
  private int current_frame;


  // Start is called before the first frame update
  void Start() {
    sr = GetComponent<SpriteRenderer>();
    sr.sprite = frames[0];
    current_frame = 0;
    next_frame_time = 0;
  }

  // Update is called once per frame
  void Update() {
    if (Time.time > next_frame_time) {
      current_frame = (current_frame + 1) % frames.Length;
      sr.sprite = frames[current_frame];
      next_frame_time = Time.time + (mapAnim.frame_interval * 1.5f);
    }
  }
}
