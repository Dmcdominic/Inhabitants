using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class string_util {
	public static string PadNumbers(string input) {
		return Regex.Replace(input, "[0-9]+", match => match.Value.PadLeft(10, '0'));
	}
}
