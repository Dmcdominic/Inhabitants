using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class holdBackEarth : MonoBehaviour {

  public float deerDelayedTime;
  public float treeDelayedTime;

  private TextMeshProUGUI TMP;


  // Init
  private void Awake() {
    TMP = GetComponent<TextMeshProUGUI>();
  }

  // Update is called once per frame
  void Update() {
    TMP.enabled = (earth_reticle.last_deer_time < Time.time - deerDelayedTime || earth_reticle.last_tree_time < Time.time - treeDelayedTime);
  }
}
