using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class keyboard_testing_input : MonoBehaviour {
  // Update is called once per frame
  void Update() {
    if (Input.GetKeyDown(KeyCode.Minus)) {
      close_game.quit();
    } else if (Input.GetKeyDown(KeyCode.Equals)) {
      SceneManager.LoadScene(0);
    }
  }
}
