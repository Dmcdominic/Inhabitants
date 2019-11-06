using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class layerOrderByY : MonoBehaviour {

  private const float y_intervals = 0.001f;
  
  private SpriteRenderer sr;


  // Init
  private void Awake() {
    sr = GetComponent<SpriteRenderer>();
  }

  // Update is called once per frame
  void Update() {
    if (transform.hasChanged) {
      sr.sortingOrder = -Mathf.RoundToInt(transform.position.y / y_intervals);
    }
  }
}
