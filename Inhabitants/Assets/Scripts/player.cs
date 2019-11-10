using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

public enum player { none, A, B, Earth };

public static class player_data {
  public static Color[] colors = new Color[4] { Color.black, Color.magenta, Color.blue, Color.green };
  public static XboxController[] controllers = new XboxController[4] { XboxController.Fourth, XboxController.First, XboxController.Second, XboxController.Third };
}
