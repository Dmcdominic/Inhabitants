using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
public class Rainstorm : MonoBehaviour {

  public float rainstorm_time = 5.0f;
  public float delta = 10f;
  public float radius = 1f;

  // Private vars
  private squish Squish;
  private float t = 0f;
  private bool ended = false;


  // Start is called before the first frame update
  void Start() {
    Squish = GetComponent<squish>();
    mixer.playSFX("rain");
  }

  // Update is called once per frame
  void Update() {
    if (ended) {
      return;
    }

    t += Time.deltaTime;
    if (t > rainstorm_time) {
      Squish.destroy_after_squish_down = true;
      Squish.squish_down();
      ended = true;
    }

    cell_controller.instance.growTrees(transform.position, radius, delta * Time.deltaTime);
    setRegionsRainFlag(true);
  }

  private void OnDestroy() {
    setRegionsRainFlag(false);
  }

  private void setRegionsRainFlag(bool newFlagVal) {
    foreach(region Region in region.allRegions) {
      if (Vector2.Distance(Region.centerpoint, transform.position) <= radius) {
        Region.gettingRainedOn = newFlagVal;
      }
    }
  }

}