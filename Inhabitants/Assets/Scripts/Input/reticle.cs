using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public class reticle : MonoBehaviour {

	// Static settings
	public static float speed_mult = 4f;
	public static float speed_cap = 3.5f;

	// Editor fields
	public XboxController controller;


    // Update is called once per frame
    void Update() {
		Vector3 velo = new Vector2(XCI.GetAxis(XboxAxis.LeftStickX, controller), XCI.GetAxis(XboxAxis.LeftStickY, controller));
		velo *= speed_mult;
		if (velo.sqrMagnitude > speed_cap) {
			velo.Normalize();
			velo *= speed_cap;
		}

		transform.position += velo * Time.deltaTime;
    }
}
