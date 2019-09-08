using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class quit_util {
	public static void quit_game() {
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
		Application.Quit();
#endif
	}
}
