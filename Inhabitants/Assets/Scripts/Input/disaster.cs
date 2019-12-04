using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disaster : MonoBehaviour {
  private const int unit_impact = 25;

  private static bool disaster_queued = false;

  public GameObject disaster_indicator;
  public GameObject earthquake_prefab;

  public static float earthquake_radius = 0.7f;
  public static disaster instance;


  private void Awake() {
    instance = this;
  }

  // Start is called before the first frame update
  void Start() {
    disaster_indicator.SetActive(disaster_queued);
    //regions = FindObjectsOfType<region>();
  }

  //Disaster logic, destroys trees and units in a region
  public static void causeDisaster(Vector2 pos, float radius) {
    Instantiate(instance.earthquake_prefab, pos, instance.earthquake_prefab.transform.rotation, null);
    cell_controller.instance.growTrees(pos, radius, -1);
    foreach (region Region in region.allRegions) {
      if (Vector2.Distance(pos, Region.centerpoint) <= radius) {
        if (Region.Owner != player.none && Region.units > unit_impact) {
          Region.units -= unit_impact;
        } else {
          Region.units = Mathf.Max(unit_impact - 5, Region.units);
          Region.Owner = player.none;
        }
      }
    }
  }

  public static bool isDisasterQueued() {
    return disaster_queued;
  }

  //Set and update indicator
  public static void setDisaster(bool mode) {
    disaster_queued = mode;
    instance.disaster_indicator.SetActive(mode);
  }
}
