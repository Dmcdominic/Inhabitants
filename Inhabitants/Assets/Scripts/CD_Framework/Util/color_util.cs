using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class color_util {
	public static void set_alpha(SpriteRenderer sr, float a) {
		Color col = sr.color;
		sr.color = new Color(col.r, col.g, col.b, a);
	}
}
