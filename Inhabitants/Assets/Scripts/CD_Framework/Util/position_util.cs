using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class position_util {
	// Set an object's x-position to a certain value
	public static void set_pos_x(Transform trans, float val, bool local) {
		if (local) {
			trans.localPosition = new Vector3(val, trans.localPosition.y, trans.localPosition.z);
		} else {
			trans.position = new Vector3(val, trans.position.y, trans.position.z);
		}
	}

	// Set an object's y-position to a certain value
	public static void set_pos_y(Transform trans, float val, bool local) {
		if (local) {
			trans.localPosition = new Vector3(trans.localPosition.x, val, trans.localPosition.z);
		} else {
			trans.position = new Vector3(trans.position.x, val, trans.position.z);
		}
	}

	// Set an object's z-position to a certain value
	public static void set_pos_z(Transform trans, float val, bool local) {
		if (local) {
			trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, val);
		} else {
			trans.position = new Vector3(trans.position.x, trans.position.y, val);
		}
	}
}
