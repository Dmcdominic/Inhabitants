using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eat_cursor : MonoBehaviour {
	public bool build_only = false;
	void Awake() {
#if UNITY_EDITOR
		if (!build_only) {
			Cursor.visible = false;
		}
#elif UNITY_STANDALONE
		Cursor.visible = false;
#endif
	}
}
