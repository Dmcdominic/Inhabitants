using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
public class Rainstorm : MonoBehaviour {

  public float rainstorm_time = 5.0f;
  public float delta = 50f;
  public float radius = 1f;


  // Start is called before the first frame update
  void Start() {
    mixer.playSFX("rain");
    Destroy(gameObject, rainstorm_time);
  }

  // Update is called once per frame
  void Update() {
    cell_controller.instance.growTrees(transform.position, radius, delta * Time.deltaTime);
  }

}