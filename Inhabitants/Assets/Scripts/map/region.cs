using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class region : MonoBehaviour {

  // Static settings
  private const float ecoRainGrowthMult = 2f;
  public const int base_units = 10;

  // Public fields
  public int area;
  public moving_units movingUnits;

  public player Owner = player.none;
  public SpriteRenderer region_sr_override;

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

  public policy Policy {
    get { return policy_manager.policies[(int)Owner]; }
  }

  [HideInInspector]
  public bool gettingRainedOn = false;

  public static List<region> allRegions = new List<region>();

  // Components
  public TextMeshPro unit_text;
  public city City;
  public GameObject raycast_orig_override;

  // Hidden components
  private SpriteRenderer ownerShade_sr;
  [HideInInspector]
  public SpriteOutline spriteOutline;
  [HideInInspector]
  public road_hub road_Hub;


  // Init
  private void Awake() {
    if (region_sr_override) {
      ownerShade_sr = region_sr_override;
    } else {
      ownerShade_sr = GetComponent<SpriteRenderer>();
    }
    spriteOutline = ownerShade_sr.gameObject.GetComponent<SpriteOutline>();

    road_Hub = GetComponent<road_hub>();
    //spriteOutline.enabled = false;
    City.Region = this;

    allRegions.Add(this);
  }

  // Start is called before the first frame update
  void Start() {
    units_real = area;
  }

  // Update is called once per frame
  void Update() {
    units_real += growth_rate * Time.deltaTime;
    if (units_real <= 0) {
      if (PlayerManager.Gamestate == gamestate.empires_falling) {
        Owner = player.none;
        units_real = 10;
      } else {
        units_real = 0;
      }
    }
    unit_text.text = units.ToString();
    unit_text.color = player_data.colors[(int)Owner];
    spriteOutline.color = player_data.colors[(int)Owner];

    // Update the owner color overlay
    if (Owner == player.A || Owner == player.B) {
      Color transparentCol = player_data.colors[(int)Owner];
      transparentCol.a = 0.2f;
      ownerShade_sr.color = transparentCol;
      ownerShade_sr.enabled = true;
    } else {
      ownerShade_sr.enabled = false;
    }
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

  public void affect_some_nearby_trees(float radius = 0.9f, float delta = 0.5f, bool forced = false) {
    if (forced) {
      cell_controller.instance.growTrees(centerpoint, radius, delta);
      return;
    }
    
    if (Policy == policy.eco) {
      cell_controller.instance.growTrees(centerpoint, radius, delta);
    } else if (Policy == policy.industry) {
      cell_controller.instance.growTrees(centerpoint, radius, -delta);
    }
  }

  // Determine the growth rate of this region
  public float growth_rate {
    get {
      if (Owner == player.none) {
        return 0;
      }
      float current_rate = Mathf.Sqrt(units_real) / 12f + 0.2f;

      float GS_mod = (PlayerManager.Gamestate == gamestate.empires_falling) ? -10f : 1f;

      switch (Policy) {
        case policy.industry:
          return current_rate * 1.2f * GS_mod;
        case policy.neutral:
          return current_rate * 1.2f * GS_mod;
        case policy.eco:
          return current_rate * (gettingRainedOn ? ecoRainGrowthMult : 1f) * GS_mod;
        default:
          return current_rate * GS_mod;
      }
    }
  }

  // The centerpoint of the region
  public Vector2 centerpoint {
    get {
      return City.transform.position;
    }
  }

  // The point on the region from which raycasts should originate
  public Vector2 raycast_centerpoint {
    get {
      return (raycast_orig_override != null) ? raycast_orig_override.transform.position : City.transform.position;
    }
  }
}
