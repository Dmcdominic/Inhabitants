using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_shaker : MonoBehaviour {

  public static camera_shaker instance;

  private Vector3 init_pos;
  private float duration;
  private float shake_timer;
  private float base_magnitude;
  private float magnitude;


  // Init
  private void Awake() {
    instance = this;
    init_pos = transform.position;
  }

  // Update is called once per frame
  void Update() {
    if (shake_timer > 0) {
      float delta_x = Random.Range(-1f, 1f) * magnitude;
      float delta_y = Random.Range(-1f, 1f) * magnitude;
      transform.position = init_pos + new Vector3(delta_x, delta_y);
      magnitude -= base_magnitude * Time.deltaTime / duration;
      shake_timer -= Time.deltaTime;
    } else {
      transform.position = init_pos;
    }
  }

  public void trigger_shake(float magnitude, float duration = 2f) {
    if (magnitude >= this.magnitude) {
      this.shake_timer = duration;
      this.duration = duration;
      this.magnitude = magnitude;
      this.base_magnitude = magnitude;
    }
  }
}
