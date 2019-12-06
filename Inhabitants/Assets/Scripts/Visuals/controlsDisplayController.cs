using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReticleControlInput;

public class controlsDisplayController : MonoBehaviour {

  public bool humans;
  public GameObject controls_display;


  // Update is called once per frame
  void Update() {
    if (PlayerManager.Gamestate == gamestate.sea_levels_rose) {
      return;
    }

    if (humans) {
      bool p1Back = RCI.GetButton(XboxCtrlrInput.XboxButton.Back, player_data.controllers[(int)player.A]);
      bool p2Back = RCI.GetButton(XboxCtrlrInput.XboxButton.Back, player_data.controllers[(int)player.B]);
      controls_display.SetActive(p1Back || p2Back);
    } else {
      if (MultiDisplayScript.singleDisplay) {
        bool p1Back = RCI.GetButton(XboxCtrlrInput.XboxButton.Back, player_data.controllers[(int)player.A]);
        bool p2Back = RCI.GetButton(XboxCtrlrInput.XboxButton.Back, player_data.controllers[(int)player.B]);
        if (!(p1Back || p2Back)) {
          controls_display.SetActive(RCI.GetButton(XboxCtrlrInput.XboxButton.Back, player_data.controllers[(int)player.Earth]));
        }
      } else {
        controls_display.SetActive(RCI.GetButton(XboxCtrlrInput.XboxButton.Back, player_data.controllers[(int)player.Earth]));
      }
    }
  }
}
