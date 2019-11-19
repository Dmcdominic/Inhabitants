using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiDisplayScript : MonoBehaviour {

  public Camera allSeeingCam;
  public Camera HumanCam;
  public Camera EarthCam;

  public Canvas playerCanvas;


  // Start is called before the first frame update
  void Start() {
    // (Theoretically) activates all connected displays
    // IMPORTANT! You cannot deactivate a display, and this must be called *once* during startup.
    foreach (Display d in Display.displays) {
      d.Activate();
    }

    // Enable/Disable cameras accordingly
    bool singleDisplay = (Display.displays.Length == 1);
    HumanCam.gameObject.SetActive(!singleDisplay);
    EarthCam.gameObject.SetActive(!singleDisplay);
    allSeeingCam.gameObject.SetActive(singleDisplay);

        playerCanvas.worldCamera = singleDisplay ? allSeeingCam : HumanCam;
  }

}
