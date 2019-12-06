using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XboxCtrlrInput;
using ReticleControlInput;

public class earth_reticle : MonoBehaviour {

  // Settings
  private static float tree_growth_radius = 0.4f;
  private static float tree_growth_rate = 0.7f;
  private const float deer_cooldown = 10f;
  private const float wolf_cooldown = 10f;

  private const float base_angular_velo = 1f;
  private const float max_angular_velo = 3.5f;
  private const float angular_accel = 1f;

  // References
  public Rainstorm rainstorm;
  public deer Deer;
  public wolf Wolf;
  public earthquake dummy_earthquake;

  // Components
  private reticle Reticle;

  // Private vars
  private float next_rainstorm = 0.0f;
  private float next_deer = 0.0f;
  private float next_wolf = 0.0f;

  private float angular_velo;

  // Static vars
  public static float last_deer_time = 0f;
  public static float last_tree_time = 0f;


  // Init
  private void Awake() {
    Reticle = GetComponent<reticle>();
  }

  // Called every frame
  private void Update() {
    dummy_earthquake.gameObject.SetActive(disaster.isDisasterQueued());
    if (disaster.isDisasterQueued()) {
      // Earthquake logic
      angular_velo = 0;
      if (RCI.GetButtonDown(XboxButton.X, Reticle.controller)) {
        disaster.causeDisaster(transform.position, disaster.earthquake_radius);
        disaster.setDisaster(false);
        dummy_earthquake.gameObject.SetActive(false);
        angular_velo = base_angular_velo;
      }
    } else if (!(PlayerManager.Gamestate == gamestate.sea_levels_rose)) {
      // Rainstorm logic
      if (RCI.GetButtonDown(XboxButton.X, Reticle.controller) && Time.time > next_rainstorm) {
        if (Physics2D.RaycastAll(transform.position, Vector2.zero).Length >= 2) {

          next_rainstorm = Time.time + rainstorm.rainstorm_time - 0.5f;
          Instantiate(rainstorm, transform.position, Quaternion.identity);
        }
      }

      // Tree-growing logic
      if (RCI.GetButton(XboxButton.A, Reticle.controller) || Input.GetKey(KeyCode.T)) {
        cell_controller.instance.growTrees(transform.position, tree_growth_radius, Time.deltaTime * tree_growth_rate);
        last_tree_time = Time.time;
        angular_velo += angular_accel * Time.deltaTime;
      } else {
        angular_velo -= angular_accel * Time.deltaTime * 1.5f;
      }
      angular_velo = Mathf.Clamp(angular_velo, base_angular_velo, max_angular_velo);

      // Animal-placing (deer) logic:
      if (RCI.GetButtonDown(XboxButton.Y, Reticle.controller) && Time.time > next_deer) {
        if (Physics2D.RaycastAll(transform.position, Vector2.zero).Length >= 2) {
          next_deer = Time.time + deer_cooldown;
          Instantiate(Deer, transform.position, Quaternion.identity);
          //int deer_total = FindObjectsOfType<deer>().Length;
          //if (deer_total < 3) {
          //}
          last_deer_time = Time.time;
        }
      }

      if (RCI.GetButtonDown(XboxButton.B, Reticle.controller)) {
        if (Physics2D.RaycastAll(transform.position, Vector2.zero).Length >= 2 && Time.time > next_wolf) {
          next_wolf = Time.time + wolf_cooldown;
          Instantiate(Wolf, transform.position, Quaternion.identity);
          //int wolf_total = FindObjectsOfType<wolf>().Length;
          //if (wolf_total < 3) {
          //}
        }
      }
    }

    // Rotate according to angular velocity
    angular_velo = Mathf.Clamp(angular_velo, 0, max_angular_velo);
    transform.Rotate(new Vector3(0, 0, angular_velo));
  }
}
