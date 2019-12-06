using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iceCaps : MonoBehaviour {
  // Settings
  public const float melt_threshold = 0.3f;

  // Components
  public List<SpriteRenderer> caps;

  // Private vars
  private int n;


  // Start is called before the first frame update
  void Start() {
    n = caps.Count - 1;
    update_caps();
  }

  // Update is called once per frame
  void Update() {
    update_caps();
  }

  // Visually update all the ice caps, based on current temperature
  private void update_caps() {
    float temp = (1f - status_controller.instance.temperatureLevel);
    for (int i=0; i < n; i++) {
      float min = melt_threshold + (i * (1f - melt_threshold) / n);
      float max = melt_threshold + ((i+1) * (1f - melt_threshold) / n);
      float alpha = Mathf.Lerp(0, 1, (max - temp) / (max - min));
      color_util.set_alpha(caps[i], alpha);
    }
  }
}
