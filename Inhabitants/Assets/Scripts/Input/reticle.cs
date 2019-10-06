using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class reticle : MonoBehaviour {

	// Static settings
	public static float speed_mult = 4f;
	public static float speed_cap = 3.5f;

    // Editor fields
    public player Owner;
	public XboxController controller;
    public LineRenderer line_to_active_region;

    // Private vars
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private region over_region;

    private region active_region;


    // Init
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.color = player_data.colors[(int)Owner];
        line_to_active_region.startColor = player_data.colors[(int)Owner];
        line_to_active_region.endColor = Color.black;
        //line_to_active_region.
    }

    // Update is called once per frame
    void Update() {
        // Update position based on controller input
<<<<<<< Updated upstream
		Vector2 velo = new Vector2(XCI.GetAxis(XboxAxis.LeftStickX, controller), XCI.GetAxis(XboxAxis.LeftStickY, controller));
		velo *= speed_mult;
=======
		//Vector2 velo = new Vector2(MoveX(), MoveY());
        Vector2 velo = new Vector2(XCI.GetAxis(XboxAxis.LeftStickX, controller), XCI.GetAxis(XboxAxis.LeftStickY, controller));
        velo *= speed_mult;
>>>>>>> Stashed changes
		if (velo.sqrMagnitude > speed_cap) {
			velo.Normalize();
			velo *= speed_cap;
		}

		rb.position += velo * Time.deltaTime;

        // The following is for human-player control, and does not apply to the Earth player
<<<<<<< Updated upstream
        if (Owner == player.Earth) {
            return;
        }

        // Update over_region using raycast, rather than trigger enter/exit
        //Physics2D.Raycast(transform.position, Vector2.up, 0.01f, )

        // TESTING
        if (XCI.GetButtonDown(XboxButton.RightBumper, controller) && over_region != null) {
=======
        if (Owner == player.Earth)
        {
            
            if (XCI.GetButtonDown(XboxButton.X, controller) && Time.time > next_rainstorm) {
                
                if (Physics2D.RaycastAll(transform.position, Vector2.zero).Length>=2)
                {
                    next_rainstorm = Time.time + rainstorm_rate;
                    Instantiate(rainstorm, transform.position, Quaternion.identity);
                }
        }
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
        if (XCI.GetButtonDown(XboxButton.RightBumper, controller )&& over_region != null) {
>>>>>>> Stashed changes
            over_region.Owner = Owner;
        }

        // Respond to button inputs
        if (XCI.GetButtonDown(XboxButton.A, controller) && over_region != null && over_region.Owner == Owner) {
            active_region = over_region;
        } else if (active_region != null && active_region.Owner != Owner) {
            active_region = null;
        } else if (!XCI.GetButton(XboxButton.A, controller)) {
<<<<<<< Updated upstream
            if (active_region != null && active_region.Owner == Owner && over_region != null && active_region != over_region) {
                active_region.send_units(over_region);
                
                
                
                // Call the "send units" function here
                // TESTING - For now, send them instantly
               /* if (active_region.units > 1) {
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
                }*/
=======
            if (active_region != null && active_region.Owner == Owner && aimed_at_region != null && active_region != aimed_at_region) {
                active_region.send_units(aimed_at_region);
>>>>>>> Stashed changes
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

    // Check when you collide with a region
    private void OnTriggerEnter2D(Collider2D collision) {
        region Region = collision.GetComponent<region>();
        if (Region != null) {
            over_region = Region;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        region Region = collision.GetComponent<region>();
        if (Region != null && Region == over_region) {
            over_region = null;
        }
    }
}
