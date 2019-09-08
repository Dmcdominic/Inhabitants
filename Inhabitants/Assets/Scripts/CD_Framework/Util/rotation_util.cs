using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class rotation_util {
	// Rotate an object to a certain x-rotation value
	public static void set_rot_x(Transform trans, float val) {
		trans.localRotation = Quaternion.Euler(new Vector3(val, trans.rotation.y, trans.rotation.z));
	}

	// Rotate an object to a certain y-rotation value
	public static void set_rot_y(Transform trans, float val) {
		trans.localRotation = Quaternion.Euler(new Vector3(trans.rotation.x, val, trans.rotation.z));
	}

	// Rotate an object to a certain z-rotation value
	public static void set_rot_z(Transform trans, float val) {
		trans.localRotation = Quaternion.Euler(new Vector3(trans.rotation.x, trans.rotation.y, val));
	}
}
