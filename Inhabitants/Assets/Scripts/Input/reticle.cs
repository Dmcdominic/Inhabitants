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

    // Private vars
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private region current_region;


    // Init
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.color = player_data.colors[(int)Owner];
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

        // Respond to button inputs
        if (XCI.GetButtonDown(XboxButton.A, controller) && current_region != null) {
            current_region.Owner = Owner;
        }
    }

    // Check when you collide with a region
    private void OnTriggerEnter2D(Collider2D collision) {
        region Region = collision.GetComponent<region>();
        if (Region != null) {
            current_region = Region;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        region Region = collision.GetComponent<region>();
        if (Region != null && Region == current_region) {
            current_region = null;
        }
    }
}
