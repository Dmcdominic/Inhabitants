using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiDisplayScript : MonoBehaviour {

  [Tooltip("If enabled, allSeeingCam will be on Display 1, HumanCam will be on Display 2, and EarthCam will be on Display 3.")]
  public bool editorDebugOverride = true;

  public Camera allSeeingCam;
  public Camera HumanCam;
  public Camera EarthCam;

  public int humanDisplay = 1;
  public int earthDisplay = 0;

  public Canvas playerCanvas;
  public Canvas earthCanvas;

  public static bool singleDisplay;


  // Start is called before the first frame update
  void Start() {
#if UNITY_EDITOR
    if (editorDebugOverride) {
      initCamerasDebug();
    } else {
      initCameras();
    }
#else
    initCameras();
#endif
  }

  // Inits cameras according to the number of displays connected
  private void initCameras() {
    // (Theoretically) activates all connected displays
    // IMPORTANT! You cannot deactivate a display, and this must be called *once* during startup.
    foreach (Display d in Display.displays) {
      d.Activate();
    }

    // Enable/Disable cameras accordingly
    singleDisplay = (Display.displays.Length == 1);

    allSeeingCam.gameObject.SetActive(singleDisplay);
    allSeeingCam.targetDisplay = 0;

    HumanCam.gameObject.SetActive(!singleDisplay);
    HumanCam.targetDisplay = humanDisplay;
    EarthCam.gameObject.SetActive(!singleDisplay);
    EarthCam.targetDisplay = earthDisplay;

    playerCanvas.worldCamera = singleDisplay ? allSeeingCam : HumanCam;
    earthCanvas.worldCamera = singleDisplay ? allSeeingCam : EarthCam;
  }

  // Inits all three cameras, to displays one, two, and three accordingly
  private void initCamerasDebug() {
    singleDisplay = false;

    // Enable/Disable cameras accordingly
    allSeeingCam.gameObject.SetActive(true);
    allSeeingCam.targetDisplay = 0;

    HumanCam.gameObject.SetActive(true);
    HumanCam.targetDisplay = 1;
    EarthCam.gameObject.SetActive(true);
    EarthCam.targetDisplay = 2;

    playerCanvas.worldCamera = HumanCam;
    earthCanvas.worldCamera = EarthCam;

    // Delete the extra audio listener on the human camera
    HumanCam.GetComponent<AudioListener>().enabled = false;
  }
}
