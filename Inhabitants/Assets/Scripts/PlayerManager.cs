using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using XboxCtrlrInput;
using ReticleControlInput;

public class PlayerManager : MonoBehaviour {
  public Canvas playerCanvas;
  public GameObject p1Reticle;
  public GameObject p2Reticle;
  public region p1StartArea;    // 0
  public region p2StartArea;    // 16

  private GameObject p1JoinText;
  private GameObject p2JoinText;

  private bool inited = false;


  // Start is called before the first frame update
  void Start() {
    p1Reticle.SetActive(false);
    p2Reticle.SetActive(false);

    p1JoinText = playerCanvas.transform.GetChild(0).gameObject;
    p2JoinText = playerCanvas.transform.GetChild(1).gameObject;
  }

  // Update is called once per frame
  void Update() {
    if (RCI.GetButton(XboxButton.Start, XboxController.First)) {
      p1Reticle.SetActive(true);
      p1JoinText.SetActive(false);
    }

    if (RCI.GetButton(XboxButton.Start, XboxController.Second)) {
      p2Reticle.SetActive(true);
      p2JoinText.SetActive(false);
    }

    if (p1Reticle.activeSelf && p2Reticle.activeSelf && !inited) {
      inited = true;
      p1StartArea.Owner = player.A;
      p2StartArea.Owner = player.B;
    }
  }
}
