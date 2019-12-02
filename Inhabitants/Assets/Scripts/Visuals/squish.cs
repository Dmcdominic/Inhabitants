using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class squish : MonoBehaviour {

  // Public fields
  public float duration = 0.5f;
  public bool squish_up_on_awake = true;
  public bool destroy_after_squish_down = true;

  // Private vars
  private Vector3 init_scale;
  private bool down = false;
  
  private float t = 0;
  private float t01 {
    get {
      return (t / duration) * (t / duration);
    }
  }


  // Start is called before the first frame update
  void Awake() {
    if (squish_up_on_awake) {
      squish_up();
    }
  }

  // Update is called once per frame
  void Update() {
    if (!down && t >= duration) {
      transform.localScale = init_scale;
      return;
    } else if (down && t <= 0) {
      transform.localScale = new Vector3(0, 0, transform.localScale.z);
      if (destroy_after_squish_down) {
        Destroy(gameObject);
      }
      return;
    }

    update_scale();
    t += Time.deltaTime * (down ? -1 : 1) ;
  }

  // Trigger a squish from scale 0 to current scale
  public void squish_up() {
    t = 0;
    down = false;


    init_scale = transform.localScale;
    update_scale();
  }
  // Trigger a squish from current scale to 0
  public void squish_down() {
    t = duration;
    down = true;

    init_scale = transform.localScale;
    update_scale();
  }

  private void update_scale() {
    float x = init_scale.x * Mathf.LerpUnclamped(t01, 1f, t01 * 2.6f);
    float y = init_scale.y * Mathf.LerpUnclamped(t01, 1f, t01 * 2.1f + 0.2f);
    transform.localScale = new Vector3(x, y, transform.localScale.z);
  }
}
