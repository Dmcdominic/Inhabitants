using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class road_hub : MonoBehaviour {

  // Component fields
  public LineRenderer road_renderer;

  // Static settings
  private static int midpt_count = 8;
  private static float visual_pulse_freq = 0.5f;
  private static float pulse_margin = 0.12f;

  // Private vars
  private int units_at_build_time;

  // Private components
  private region source;
  private region dest;


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
      //destroy_road(); // TODO - uncomment
      return;
    }

    // Send any excess units
    int unit_diff = source.units - units_at_build_time;
    if (unit_diff > 0) {
      source.send_n_units(dest, unit_diff);
    }

    // Animate the road renderer
    float t = ((Time.time * visual_pulse_freq) % 1) * (1f - 2f*pulse_margin) + pulse_margin;

    Keyframe zero = road_renderer.widthCurve.keys[0];
    Keyframe last = road_renderer.widthCurve.keys[road_renderer.widthCurve.keys.Length - 1];
    Keyframe mid0 = new Keyframe(Mathf.Clamp01(t - pulse_margin), zero.value);
    Keyframe mid1 = new Keyframe(t, 1f);
    Keyframe mid2 = new Keyframe(Mathf.Clamp01(t + pulse_margin), last.value);

    AnimationCurve widthCurve = new AnimationCurve();
    widthCurve.keys = new Keyframe[5] { zero, mid0, mid1, mid2, last };
    road_renderer.widthCurve = widthCurve;
  }

  // Build a road
  public void build_road(region target) {
    if (target.Owner != source.Owner) {
      // Can't build road to other player's region/city
      return;
    }

    dest = target;
    units_at_build_time = source.units;

    // Build the road (visually)
    road_renderer.positionCount = midpt_count + 2;
    road_renderer.SetPositions(midpoints(source.centerpoint, dest.centerpoint, midpt_count));
    road_renderer.enabled = true;

    // Todo - destroy trees between two regions
  }

  // Destroy a road
  public void destroy_road() {
    dest = null;

    // Destroy the outgoing road (visually)
    road_renderer.enabled = false;
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
