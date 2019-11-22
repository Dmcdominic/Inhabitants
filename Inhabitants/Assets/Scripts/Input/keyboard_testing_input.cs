using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class keyboard_testing_input : MonoBehaviour {

  // Public fields
  public float hold_esc_to_quit = 0.5f;

  // Private vars
  private float escape_timer;


  // Update is called once per frame
  void Update() {
    // '0' to double timescale. '9' to cut it in half.
    if (Input.GetKeyDown(KeyCode.Alpha0)) {
      Time.timeScale *= 2f;
    } else if (Input.GetKeyDown(KeyCode.Alpha9)) {
      Time.timeScale /= 2f;
    }

    // '-' to quit. '=' to reload scene.
    // In build: 'r' to reload scene.
    if (Input.GetKeyDown(KeyCode.Minus)) {
      close_game.quit();
#if UNITY_EDITOR
    } else if (Input.GetKeyDown(KeyCode.Equals)) {
#endif
#if UNITY_STANDALONE
    } else if (Input.GetKeyDown(KeyCode.R)) {
#endif
      SceneManager.LoadScene(0);
    }
  }


  private void FixedUpdate() {
    // Hold escape to quit
    if (Input.GetKey(KeyCode.Escape)) {
      escape_timer += Time.fixedDeltaTime;
      if (escape_timer >= hold_esc_to_quit) {
        close_game.quit();
      }
    } else {
      escape_timer = 0;
    }
  }
}
