using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class road_hub : MonoBehaviour {

  // Component fields
  public LineRenderer road_renderer;

  // Static settings
  private const int midpt_count = 8;
  /* private const float visual_pulse_freq = 0.5f;
  private const float pulse_margin = 0.12f;*/
  private static Vector2 midpt_offset = new Vector2(0, -0.12f);

  private static float tree_destruction_radius = 0.1f;
  private static float on_build_tree_delta = -1f;

  // Private vars
  private int units_cap;
  private Vector3[] current_midpoints;

  // Private components
  private region source;
  private region dest;

  public bool road_active {
   get { return dest != null; }
  }


  // Init
  private void Awake() {
    source = GetComponent<region>();
    road_renderer.enabled = false;
  }

  // Update is called once per frame
  void Update() {
    if (dest == null) {
      return;
    }

    if (source.Owner == player.none || source.Owner != dest.Owner) {
      destroy_road();
      return;
    }

    // Send any excess units
    send_excess_units();
    units_cap = source.units;

    // Animate the road renderer
    /*
    float t = ((Time.time * visual_pulse_freq) % 1) * (1f - 2f*pulse_margin) + pulse_margin;

    Keyframe zero = road_renderer.widthCurve.keys[0];
    Keyframe last = road_renderer.widthCurve.keys[road_renderer.widthCurve.keys.Length - 1];
    Keyframe mid0 = new Keyframe(Mathf.Clamp01(t - pulse_margin), zero.value);
    Keyframe mid1 = new Keyframe(t, 2f);
    Keyframe mid2 = new Keyframe(Mathf.Clamp01(t + pulse_margin), last.value);

    AnimationCurve widthCurve = new AnimationCurve();
    widthCurve.keys = new Keyframe[5] { zero, mid0, mid1, mid2, last };
    road_renderer.widthCurve = widthCurve; */
  }

  // Build a road
  public void build_road(region target) {
    if (target.Owner != source.Owner) {
      // Can't build road to other player's region/city
      return;
    }

    // If a road already exists, destroy it
    if (dest != null) {
      bool no_build = target == dest;
      destroy_road();
      if (no_build) return;
    }

    // Play sound effect
    mixer.playSFX("road");

    dest = target;
    units_cap = source.units;

    // Destroy the opposite road, if it exists
    if (dest.road_Hub.dest == source) {
      dest.road_Hub.destroy_road();
    }

    // Build the road (visually)
    road_renderer.positionCount = midpt_count + 2;
    current_midpoints = midpoints(source.centerpoint + midpt_offset, dest.centerpoint + midpt_offset, midpt_count);
    road_renderer.SetPositions(current_midpoints);
    road_renderer.enabled = true;

    // Destroy trees along the road
    if (source.Policy != policy.eco) {
      foreach (Vector3 pos in current_midpoints) {
        cell_controller.instance.growTrees(pos, tree_destruction_radius, on_build_tree_delta);
      }
    }
  }

  // Destroy a road
  public void destroy_road() {
    dest = null;

    // Destroy the outgoing road (visually)
    road_renderer.enabled = false;
  }

  // Send any excess units that should be sent
  public void send_excess_units() {
    if (road_active) {
      int unit_diff = source.units - units_cap;
      if (unit_diff > 0) {
        source.send_n_units(dest, unit_diff);
      }
    }
  }
  
  // Generates a Vector3[] of points between source and destination
  private Vector3[] midpoints(Vector3 start, Vector3 end, int count) {
    Vector3[] points = new Vector3[count + 2];
    points[0] = start;

    for (int i = 1; i < count + 1; i++) {
      points[i] = Vector3.Lerp(start, end, (float)i / (count + 1));
    }
    points[count + 1] = end;
    
    return points;
  }
}
