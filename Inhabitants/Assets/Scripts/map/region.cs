using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class region : MonoBehaviour {

  // Public fields
  public int area;
  public moving_units movingUnits;

  public player Owner = player.none;

  // Public variables
  public int units {
    get { return Mathf.FloorToInt(units_real); }
    set { units_real = value; }
  }
  protected float units_real {
    get { return _units_real; }
    set { _units_real = value; road_Hub.send_excess_units(); }
  }
  protected float _units_real;

  // Components
  public TextMeshPro unit_text;
  public city City;

  // Hidden components
  [HideInInspector]
  public SpriteOutline spriteOutline;
  [HideInInspector]
  public road_hub road_Hub;


  // Init
  private void Awake() {
    spriteOutline = GetComponent<SpriteOutline>();
    road_Hub = GetComponent<road_hub>();
    //spriteOutline.enabled = false;
  }

  // Start is called before the first frame update
  void Start() {
    units_real = area;
  }

  // Update is called once per frame
  void Update() {
    units_real += growth_rate * Time.deltaTime;
    unit_text.text = units.ToString();
    unit_text.color = player_data.colors[(int)Owner];
    spriteOutline.color = player_data.colors[(int)Owner];
  }

  public void send_units(region region_target) {
    int unit_to_send = units / 2;

    if (unit_to_send == 0) {
      //doesn't have enough units
      // TODO - negative feedback?
    } else {
      send_n_units(region_target, unit_to_send);
    }
  }

  public void send_n_units(region region_target, int num_units) {
    if (units <= 0) {
      Debug.LogError("Tried to send " + units + " units");
      return;
    }

    //instantiate the moving units first
    moving_units x = Instantiate(movingUnits, centerpoint, movingUnits.transform.rotation);
    x.target_region = region_target;
    x.start_owner = Owner;
    x.start_position = transform.position;
    x.units = num_units;
    units -= num_units;
    //assign the proper parameters
  }

  // Build a road
  public void build_road(region target) {
    road_Hub.build_road(target);
  }

  public void clear_some_nearby_trees(float radius = 0.9f, float delta = -0.5f) {
    cell_controller.instance.growTrees(centerpoint, radius, delta);
  }

  // Determine the growth rate of this region
  public float growth_rate {
    get {
      if (Owner == player.none) {
        return 0;
      }
      float current_rate = Mathf.Sqrt(units_real) / 12f + 0.2f;
      return current_rate;
    }
  }

  // The centerpoint of the region
  public Vector2 centerpoint {
    get {
      return City.transform.position;
    }
  }
}
