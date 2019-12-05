using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sea_level_controller : MonoBehaviour {

  // Constant settings
  private const float wave_speed = 5f;
  private const float wave_scale_rate = 0.04f;
  private const float max_scale_ratio = 2.5f;

  // Public fields
  public wave_cycles wave;
  public GameObject sea;
  public GameObject sea_mask;

#if UNITY_EDITOR
  public static bool test_on_start = false;
#else
  public static bool test_on_start = false;
#endif

  // Private vars
  private Vector3 wave_init_pos;
  private Vector3 wave_init_scale;

  // Static vars
  private static bool risen = false;
  public static bool idle = false;


  // Start is called before the first frame update
  void Start() {
    wave_init_pos = wave.transform.position;
    wave_init_scale = wave.transform.localScale;
    reset();

    if (test_on_start) {
      risen = true;
      wave.enabled = true;
    }
  }

  // Update is called once per frame
  void Update() {

    if (!risen && PlayerManager.Gamestate == gamestate.sea_levels_rose) {
      risen = true;
      wave.enabled = true;
    } else if (risen && PlayerManager.Gamestate != gamestate.sea_levels_rose && !test_on_start) {
      reset();
      // TODO - fade out the sea?
      return;
    }

    if (risen && !idle) {
      Vector3 movement = Vector3.left * wave_speed * Time.deltaTime;
      wave.transform.position += movement;

      float scale_factor = (wave_init_scale.x * max_scale_ratio - wave.transform.localScale.x) / wave_init_scale.x;
      wave.transform.localScale += Vector3.one * wave_scale_rate * scale_factor * Time.deltaTime;

      if (sea_mask.transform.position.x < 0) {
        idle = true;
        wave.enabled = false;
      }
    }
  }

  // Reset the wave and sea level
  private void reset() {
    wave.transform.position = wave_init_pos;
    wave.transform.localScale = wave_init_scale;
    wave.enabled = false;
    risen = false;
    idle = false;
  }
}
