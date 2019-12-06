using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using ReticleControlInput;

public class reticle : MonoBehaviour {

  // Static settings
  private static float speed_mult = 4f;
  private static float speed_cap = 3.5f;
  private static float earth_speed_adj = 0.65f;

  private static float raycast_radius = 0.18f;
  private static float rTrigger_thresh = 0.25f;

  // Editor fields
  public player Owner;
  public LineRenderer line_to_active_region;
  public SpriteRenderer arrow_cap;

  public Sprite closed_cursor;
  public Sprite open_cursor;

  // Hidden vars
  [HideInInspector]
  public XboxController controller;

  // Private vars
  private Rigidbody2D rb;
  private SpriteRenderer sr;

  private region over_region;
  private region active_region;
  private region aimed_at_region;
  private earth_reticle earth_Reticle;
  private Dictionary<region, int> touching_regions = new Dictionary<region, int>();
  private static LayerMask region_mask;
  private static ContactFilter2D contactFilter = new ContactFilter2D();
  private bool rTrigger_down_prev = false;
  
  private float speed_adj {
    get { return Owner == player.Earth ? earth_speed_adj : 1f; }
  }


  // Init
  private void Awake() {
    rb = GetComponent<Rigidbody2D>();
    sr = GetComponent<SpriteRenderer>();
    controller = player_data.controllers[(int)Owner];
    earth_Reticle = GetComponent<earth_reticle>();
    if (Owner != player.Earth) {
      earth_Reticle.enabled = false;
    }

    // Init visuals
    //Color fadedCol = player_data.colors[(int)Owner];
    //fadedCol.a = sr.color.a;
    //sr.color = fadedCol;

    arrow_cap.color = player_data.colors[(int)Owner];
    line_to_active_region.startColor = player_data.colors[(int)Owner];
    line_to_active_region.endColor = player_data.colors[(int)Owner];
    line_to_active_region.enabled = false;
    arrow_cap.enabled = false;

    // Init raycast contact filter
    region_mask = LayerMask.GetMask(new string[] { "Regions" });
    contactFilter.NoFilter();
    contactFilter.useLayerMask = true;
    contactFilter.layerMask = region_mask;
  }

  // Update is called once per frame
  void Update() {
    // Update position based on controller input
    Vector2 velo = new Vector2(RCI.GetAxis(XboxAxis.LeftStickX, controller), RCI.GetAxis(XboxAxis.LeftStickY, controller));

    velo *= speed_mult * speed_adj;
    if (velo.sqrMagnitude > speed_cap * speed_adj) {
      velo.Normalize();
      velo *= speed_cap * speed_adj;
    }
    if (active_region == null) {
      rb.position += velo * Time.deltaTime;
    }

    // Clamp the reticle within the camera bounds
    Vector2 cam_min = Camera.main.ViewportToWorldPoint(new Vector3(0, 0));
    Vector2 cam_max = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f));
    float new_x = Mathf.Clamp(rb.position.x, cam_min.x, cam_max.x);
    float new_y = Mathf.Clamp(rb.position.y, cam_min.y, cam_max.y);
    rb.position = new Vector2(new_x, new_y);

    // The following is for human-player control, and does not apply to the Earth player
    if (Owner == player.Earth) {
      return;
    }

    Vector2 rightStickAim = new Vector2(RCI.GetAxis(XboxAxis.RightStickX, controller), RCI.GetAxis(XboxAxis.RightStickY, controller));

    // Update over_region using raycast, rather than trigger enter/exit
    update_over_region();
    sr.sprite = (over_region != null && over_region.Owner == Owner) ? open_cursor : closed_cursor;

    // Update active_region
    if (rightStickAim.magnitude != 0 && over_region != null && over_region.Owner == Owner) {
      active_region = over_region;
    } else if (active_region != null && active_region.Owner != Owner) {
      active_region = null;
    } else if (rightStickAim.magnitude == 0) {
      active_region = null;
    }

    // Update aimed_at region
    sr.enabled = (active_region == null);
    if (active_region != null) {
      // TODO - Highlight active region here
      //transform.position = active_region.centerpoint;
      if (rightStickAim.magnitude != 0) {
        aimed_at_region = raycast_to_region(active_region.raycast_centerpoint, rightStickAim, active_region.gameObject);
      } else {
        aimed_at_region = null;
      }
    } else {
      aimed_at_region = null;
    }

    // Send units
    bool sendButtonHeld = RCI.GetAxis(XboxAxis.RightTrigger, controller) >= rTrigger_thresh;
    bool sendButtonDown = sendButtonHeld && !rTrigger_down_prev;
    rTrigger_down_prev = sendButtonHeld;

    if (sendButtonDown && active_region != null && aimed_at_region != null) {
      if (active_region != aimed_at_region) {
        active_region.send_units(aimed_at_region);
      }
    }

    // Build a road
    bool buildRoadButtonDown = RCI.GetButtonDown(XboxButton.RightBumper, controller);
    if (buildRoadButtonDown && active_region != null && aimed_at_region != null) {
      if (active_region != aimed_at_region && aimed_at_region.Owner == Owner) {
        active_region.build_road(aimed_at_region);
      }
    }

    // Clear this road
    bool clearRoadButtonDown = RCI.GetButtonDown(XboxButton.LeftBumper, controller);
    if (clearRoadButtonDown && over_region != null) {
      if (over_region.Owner == Owner) {
        over_region.road_Hub.destroy_road();
      }
    }

    // Update the visual line to active region
    bool show_line = (active_region != null && aimed_at_region != null);
    line_to_active_region.enabled = show_line;
    arrow_cap.enabled = show_line;
    if (show_line) {
      Vector2 line_start_pos = active_region.centerpoint;
      Vector2 line_end_pos = aimed_at_region.centerpoint;
      Vector2 main_dir = line_end_pos - line_start_pos;
      Vector2 perp_dir = new Vector2(-main_dir.y, main_dir.x).normalized;
      Vector2 line_mid_pos = (line_end_pos - line_start_pos) / 2f + line_start_pos + perp_dir * 0.05f * main_dir.magnitude;
      line_to_active_region.SetPositions(new Vector3[] { line_start_pos, line_mid_pos, line_end_pos });
      arrow_cap.transform.position = line_end_pos;
      arrow_cap.transform.right = (Vector2)arrow_cap.transform.position - line_mid_pos;
    }
  }

  // Sets over_region to the region that the reticle is currently hovering over
  private void update_over_region() {
    if (active_region != null) {
      over_region = active_region;
      return;
    }
    RaycastHit2D[] hits = { raycast_in_dir(new Vector2( 1,  1)),
                                raycast_in_dir(new Vector2( 1, -1)),
                                raycast_in_dir(new Vector2(-1,  1)),
                                raycast_in_dir(new Vector2(-1, -1)),
                                raycast_in_dir(new Vector2( 1,  0)),
                                raycast_in_dir(new Vector2(-1,  0)),
                                raycast_in_dir(new Vector2( 0,  1)),
                                raycast_in_dir(new Vector2( 0, -1)) };

    // Count the hits for each region
    touching_regions.Clear();
    for (int i = 0; i < hits.Length; i++) {
      if (hits[i].collider != null) {
        region Region = hits[i].collider.GetComponent<region>();
        if (Region != null) {
          if (touching_regions.ContainsKey(Region)) {
            touching_regions[Region]++;
          } else {
            touching_regions[Region] = 1;
          }
        }
        // Otherwise, this is a "blocker" region
      }
    }

    // Determine which region has the most hits
    over_region = (active_region != null && touching_regions.ContainsKey(active_region)) ? active_region : null;
    int max_hits = 0;
    foreach (region Region in touching_regions.Keys) {
      if (Region != active_region && touching_regions[Region] > max_hits) {
        max_hits = touching_regions[Region];
        over_region = Region;
      }
    }
  }

  private RaycastHit2D raycast_in_dir(Vector2 dir) {
    return Physics2D.Raycast((Vector2)transform.position + dir.normalized * raycast_radius * 0.5f, dir, raycast_radius * 0.5f, region_mask);
  }

  // Returns the nearest region in the "dir" direction. Returns null if none exists
  private static region raycast_to_region(Vector2 src, Vector2 dir, GameObject exclude) {
    List<RaycastHit2D> results = new List<RaycastHit2D>();
    Physics2D.Raycast(src, dir, contactFilter, results);

    GameObject closest = null;
    float min_dist = float.MaxValue;
    foreach (RaycastHit2D hit in results) {
      if (hit.collider != null && hit.collider.gameObject != exclude && hit.distance < min_dist) {
        closest = hit.collider.gameObject;
        min_dist = hit.distance;
      }
    }

    if (closest != null) {
      region hit_region = closest.GetComponent<region>();
      if (hit_region != null) {
        return hit_region;
      }
    }

    // Otherwise, we hit nothing, or a "blocker" region
    return null;
  }

}
