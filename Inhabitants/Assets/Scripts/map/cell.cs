using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cell : MonoBehaviour {

  private float _state;


  public float state {
    get { return _state; }
    set {
      _state = value;
      if (state < 0.3) {
        Vector3 scale = new Vector3(0.0f, 0.0f, 1.0f);
        transform.localScale = scale;
      } else {
        Vector3 scale = new Vector3((state - 0.3f) / 0.7f, (state - 0.3f) / 0.7f, 1.0f);
        transform.localScale = scale;
      }
    }
  }
}