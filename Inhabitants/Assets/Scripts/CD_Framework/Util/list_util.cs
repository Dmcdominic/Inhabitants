using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class list_util {
	// Randomly shuffles a list, in place
	public static void shuffle<T>(List<T> list) {
		for (int i=0; i < list.Count - 1; i++) {
			int chosen_index = Random.Range(i, list.Count);
			T prev = list[i];
			list[i] = list[chosen_index];
			list[chosen_index] = prev;
		}
	}
}
