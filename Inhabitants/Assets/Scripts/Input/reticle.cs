using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class reticle : MonoBehaviour {

	// Static settings
	private static float speed_mult = 4f;
	private static float speed_cap = 3.5f;
    private static float raycast_radius = 0.18f;

    // Editor fields
    public player Owner;
	public XboxController controller;
    public LineRenderer line_to_active_region;

    // Private vars
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private region over_region;
    private region active_region;
    private region aimed_at_region;
    private Dictionary<region, int> touching_regions = new Dictionary<region, int>();
    private static LayerMask region_mask;
    private static ContactFilter2D contactFilter = new ContactFilter2D();


    // Init
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.color = player_data.colors[(int)Owner];
        line_to_active_region.startColor = player_data.colors[(int)Owner];
        line_to_active_region.endColor = Color.black;
        //line_to_active_region.
        region_mask = LayerMask.GetMask(new string[] { "Regions" });
        contactFilter.NoFilter();
        contactFilter.useLayerMask = true;
        contactFilter.layerMask = region_mask;
    }

    // Update is called once per frame
    void Update() {
        // Update position based on controller input
		Vector2 velo = new Vector2(XCI.GetAxis(XboxAxis.LeftStickX, controller), XCI.GetAxis(XboxAxis.LeftStickY, controller));
		velo *= speed_mult;
		if (velo.sqrMagnitude > speed_cap) {
			velo.Normalize();
			velo *= speed_cap;
		}

		rb.position += velo * Time.deltaTime;

        // The following is for human-player control, and does not apply to the Earth player
        if (Owner == player.Earth) {
            return;
        }

        // Update over_region using raycast, rather than trigger enter/exit
        update_over_region();

        // Update aimed_at region
        sr.enabled = (active_region == null);
        if (active_region != null) {
            // TODO - Highlight active region here
            transform.position = active_region.centerpoint;
            Vector2 leftStickAim = new Vector2(XCI.GetAxis(XboxAxis.LeftStickX, controller), XCI.GetAxis(XboxAxis.LeftStickY, controller));
            if (leftStickAim.magnitude != 0) {
                region new_aiming_at = raycast_to_region(leftStickAim);
                aimed_at_region = new_aiming_at;
            } else {
                aimed_at_region = null;
            }
        } else {
            aimed_at_region = null;
        }

        // TESTING
        if (XCI.GetButtonDown(XboxButton.RightBumper, controller) && over_region != null) {
            over_region.Owner = Owner;
        } else if (XCI.GetButtonDown(XboxButton.LeftBumper, controller) && over_region != null) {
            over_region.Owner = player.none;
        }

        // Respond to button inputs
        if (XCI.GetButtonDown(XboxButton.A, controller) && over_region != null && over_region.Owner == Owner) {
            active_region = over_region;
        } else if (active_region != null && active_region.Owner != Owner) {
            active_region = null;
        } else if (!XCI.GetButton(XboxButton.A, controller)) {
            if (active_region != null && active_region.Owner == Owner && aimed_at_region != null && active_region != aimed_at_region) {
                active_region.send_units(aimed_at_region);
            }
            active_region = null;
        }

        line_to_active_region.enabled = (active_region != null && aimed_at_region != null);
        if (active_region != null && aimed_at_region != null) {
            Vector2 line_start_pos = active_region.centerpoint;
            //Vector2 line_end_pos = (aimed_at_region != null && aimed_at_region != active_region) ? aimed_at_region.centerpoint : transform.position;
            Vector2 line_end_pos = aimed_at_region.centerpoint;
            Vector2 main_dir = line_end_pos - line_start_pos;
            Vector2 perp_dir = new Vector2(-main_dir.y, main_dir.x).normalized;
            Vector2 line_mid_pos = (line_end_pos - line_start_pos) / 2f + line_start_pos + perp_dir * 0.05f * main_dir.magnitude;
            line_to_active_region.SetPositions(new Vector3[] { line_start_pos, line_mid_pos, line_end_pos });
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
                } else {
                    Debug.LogError("Object in Regions layer hit, but does not have region component");
                }
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
        return Physics2D.Raycast(transform.position + (Vector3)dir.normalized * raycast_radius * 0.5f, dir, raycast_radius * 0.5f, region_mask);
    }

    // Returns the nearest region in the "dir" direction. Returns null if none exists
    private region raycast_to_region(Vector2 dir) {
        List<RaycastHit2D> results = new List<RaycastHit2D>();
        Physics2D.Raycast(transform.position, dir, contactFilter, results);

        region closest = null;
        float min_dist = float.MaxValue;
        foreach (RaycastHit2D hit in results) {
            if (hit.collider != null && hit.collider.gameObject != active_region.gameObject && hit.distance < min_dist) {
                region hit_region = hit.collider.GetComponent<region>();
                if (hit_region != null) {
                    closest = hit_region;
                    min_dist = hit.distance;
                } else {
                    Debug.LogError("raycast_to_region hit an object that does not contain the region component.");
                }
            }
        }
        return closest;
    }
}
