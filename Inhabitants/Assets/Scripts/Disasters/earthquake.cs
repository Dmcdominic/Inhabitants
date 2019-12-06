using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class earthquake : MonoBehaviour {

  // Settings
  public bool dummy_reticle_quake = false;

  private const float shake_radius = 0.07f;
  private const float alpha_range = 0.5f;
  private const float fade_time = 4f;

  private const float dummy_shake_dampen = 0.2f;

  public SpriteRenderer sr;

  // Private vars
  private Vector2 init_pos;
  private float timer = 0f;

  private bool minor_shake = false;


  // Start is called before the first frame update
  void Start() {
    timer = 0f;
    init_pos = transform.localPosition;
    if (!dummy_reticle_quake) {
      mixer.playSFX("earthquake");
      camera_shaker.instance.trigger_shake(0.23f, fade_time - 1f);
    }
  }

  // Reset minor shake for dummy reticle quakes
  private void OnEnable() {
    minor_shake = false;
  }

  // Update is called once per frame
  void Update() {
    // Make sure rotation isn't off
    transform.rotation = Quaternion.identity;

    // Update position
    float rand_x = Random.Range(-1f, 1f) * shake_radius * (dummy_reticle_quake ? dummy_shake_dampen : 1f);
    float rand_y = Random.Range(-1f, 1f) * shake_radius * (dummy_reticle_quake ? dummy_shake_dampen : 1f);
    transform.localPosition = new Vector2(init_pos.x + rand_x, init_pos.y + rand_y);

    if (!dummy_reticle_quake) {
      // Check if we should destroy this
      timer += Time.deltaTime;
      if (timer >= fade_time) {
        Destroy(gameObject);
      }
    } else if (!minor_shake) {
      minor_shake = true;
      camera_shaker.instance.trigger_shake(0.015f, fade_time * 0.4f);
    }

    // Update sr
    float base_alpha = (fade_time - timer) / fade_time;
    float rand_alpha = Random.Range(1f - alpha_range, 1f);
    float alpha = Mathf.Clamp(base_alpha * rand_alpha, 0f, 1f);
    color_util.set_alpha(sr, alpha);
  }
}
