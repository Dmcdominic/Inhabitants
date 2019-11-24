using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;

namespace ReticleControlInput {
  public static class RCI {
    /*
     * INPUT VALUES IN INPUTMANAGER FOR PLAYER #:
     * 
     * Left Joystick:   P#LeftStickX
     * Right Joystick:  P#RightStickX
     * 
     */

    public static bool GetButton(XboxButton button, XboxController controller) {
#if UNITY_EDITOR
      int controllerNum = Ctrl2Num(controller);

      if (XCI.GetNumPluggedCtrlrs() >= controllerNum) return XCI.GetButton(button, controller);
      else return Input.GetButton("P" + controllerNum + button);
#else
      return XCI.GetButton(button, controller);
#endif
    }

    public static bool GetButtonDown(XboxButton button, XboxController controller) {
#if UNITY_EDITOR
      int controllerNum = Ctrl2Num(controller);

      if (XCI.GetNumPluggedCtrlrs() >= controllerNum) return XCI.GetButtonDown(button, controller);
      else return Input.GetButtonDown("P" + controllerNum + button);
#else
      return XCI.GetButtonDown(button, controller);
#endif
    }

    public static float GetAxis(XboxAxis axis, XboxController controller) {
#if UNITY_EDITOR
      int controllerNum = Ctrl2Num(controller);

      if (XCI.GetNumPluggedCtrlrs() >= controllerNum) return XCI.GetAxis(axis, controller);
      else return Input.GetAxis("P" + controllerNum + axis);
#else
      return XCI.GetAxis(axis, controller);
#endif
    }

    private static int Ctrl2Num(XboxController controller) {
      if (controller == XboxController.First) return 1;
      else if (controller == XboxController.Second) return 2;
      else if (controller == XboxController.Third) return 3;
      else return 4;
    }
  }
}
