using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class earthquake : MonoBehaviour {

  // Settings
  public float shake_radius = 0.07f;
  public float alpha_range = 0.3f;
  public float fade_time = 4f;

  public SpriteRenderer sr;

  // Private vars
  private Vector2 init_pos;
  private float timer = 0f;


  // Start is called before the first frame update
  void Start() {
    init_pos = transform.position;
    mixer.playSFX("earthquake");
    // TODO - camera shake
  }

  // Update is called once per frame
  void Update() {
    // Update position
    float rand_x = Random.Range(-1f, 1f) * shake_radius;
    float rand_y = Random.Range(-1f, 1f) * shake_radius;
    transform.position = new Vector2(init_pos.x + rand_x, init_pos.y + rand_y);

    // Update sr
    timer += Time.deltaTime;
    float base_alpha = (fade_time - timer) / fade_time;
    float rand_alpha = Random.Range(1f - alpha_range, 1f);
    float alpha = Mathf.Clamp(base_alpha * rand_alpha, 0f, 1f);
    color_util.set_alpha(sr, alpha);

    // Check if we should destroy this
    if (timer >= fade_time) {
      Destroy(gameObject);
    }
  }
}
