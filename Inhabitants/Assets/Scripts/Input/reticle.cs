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
    private Dictionary<region, int> touching_regions = new Dictionary<region, int>();
    private static LayerMask region_mask;


    // Init
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.color = player_data.colors[(int)Owner];
        line_to_active_region.startColor = player_data.colors[(int)Owner];
        line_to_active_region.endColor = Color.black;
        //line_to_active_region.
        region_mask = LayerMask.GetMask(new string[] { "Regions" });
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

        // TESTING
        if (XCI.GetButtonDown(XboxButton.RightBumper, controller) && over_region != null) {
            over_region.Owner = Owner;
        }

        // Respond to button inputs
        if (XCI.GetButtonDown(XboxButton.A, controller) && over_region != null && over_region.Owner == Owner) {
            active_region = over_region;
        } else if (active_region != null && active_region.Owner != Owner) {
            active_region = null;
        } else if (!XCI.GetButton(XboxButton.A, controller)) {
            if (active_region != null && active_region.Owner == Owner && over_region != null && active_region != over_region) {
                // Call the "send units" function here
                // TESTING - For now, send them instantly
                if (active_region.units > 1) {
                    int units_to_send = active_region.units / 2;
                    active_region.units -= units_to_send;
                    if (over_region.Owner == Owner) {
                        over_region.units += units_to_send;
                    } else {
                        over_region.units -= units_to_send;
                        if (over_region.units < 0) {
                            over_region.units *= -1;
                            over_region.Owner = Owner;
                        } else if (over_region.units == 0) {
                            over_region.Owner = player.none;
                        }
                    }
                }
            }
            active_region = null;
        }

        line_to_active_region.enabled = (active_region != null);
        if (active_region != null) {
            Vector2 line_start_pos = active_region.transform.position;
            Vector2 line_end_pos = (over_region != null && over_region != active_region) ? over_region.transform.position : transform.position;
            Vector2 main_dir = line_end_pos - line_start_pos;
            Vector2 perp_dir = new Vector2(-main_dir.y, main_dir.x).normalized;
            Vector2 line_mid_pos = (line_end_pos - line_start_pos) / 2f + line_start_pos + perp_dir * 0.05f * main_dir.magnitude;
            line_to_active_region.SetPositions(new Vector3[] { line_start_pos, line_mid_pos, line_end_pos });
        }
    }

    private void update_over_region() {
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
}
