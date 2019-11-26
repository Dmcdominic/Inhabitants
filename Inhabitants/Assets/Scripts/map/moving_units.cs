using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class moving_units : MonoBehaviour {
  // Static settings
  private const float movespeed = 0.25f;
  private const float sr_radius = 0.35f;
  private const float arrival_radius = 0.25f;

  private const float tree_effect_odds = 0.04f;
  private const float tree_effect_radius = 0.35f;
  private const float tree_destr_delta = -0.35f, tree_grow_delta = 0.4f;

  // Components
  public region target_region;
  public player start_owner;
  public Vector3 start_position;
  public int units;

  public TextMeshPro units_num;
  public SpriteRenderer single_sr;

  // Static vars
  // TODO - bool to check if each player has any units
  public static bool pA_has_units;
  public static bool pB_has_units;

  // Private vars
  private int single_unit_sr_total = 1;
  private bool dummy_units = false;

  // Getters
  public policy Policy {
    get { return policy_manager.policies[(int)start_owner]; }
  }


  // Start is called before the first frame update
  void Start() {
    single_sr.GetComponent<Animator>().SetBool("blue", start_owner == player.B);
    Vector2 direction = target_region.centerpoint - (Vector2)transform.position;
    if (direction.x > 0) {
      Vector3 lEA = single_sr.transform.localEulerAngles;
      single_sr.transform.localEulerAngles = new Vector3(lEA.x, 0, lEA.z);
    }

    // Set up a number of redcoat sprites depending on unit count
    // Thresholds increase cubically
    int threshold = 2;
    while (units > threshold * threshold * threshold) {
      addSingleUnitSR();
      threshold += 1;
    }

    // Set the has_units bool
    if (start_owner == player.A) {
      pA_has_units = true;
    } else if (start_owner == player.B) {
      pB_has_units = true;
    }
  }

  // Update is called once per frame
  void Update() {
    if (PlayerManager.Gamestate == gamestate.empires_falling) {
      dummy_units = true;
    }

    float ds = Time.deltaTime * movespeed;

    units_num.text = units.ToString();
    units_num.color = player_data.colors[(int)start_owner];

    //checks if the # of units moving has gone down to zero. Terminate if so.
    if (units <= 0) {
      Destroy(this.gameObject);
      return;
    }

    // Small odds to affect nearby trees
    for (int i=0; i < single_unit_sr_total*single_unit_sr_total; i++) {
      float randFloat = Random.Range(0, 1f);
      if (randFloat < tree_effect_odds * Time.deltaTime) {
        if (Policy == policy.industry) {
          cell_controller.instance.growTrees(transform.position, tree_effect_radius, tree_destr_delta);
        } else if (Policy == policy.eco) {
          cell_controller.instance.growTrees(transform.position, tree_effect_radius, tree_grow_delta);
        }
        break;
      }
    }

    // If we're not close enough, move the units
    float dist = Vector2.Distance(transform.position, target_region.centerpoint);
    if (dist > ds && dist > arrival_radius) {
      Vector2 direction = target_region.centerpoint - (Vector2)transform.position;
      transform.Translate(direction.normalized * ds);
      return;
    }

    if (dummy_units) {
      Destroy(gameObject);
      return;
    }

    //check if the target region is friendly, if yes
    if (target_region.Owner == start_owner) {
      target_region.units += units;
    } else {
      // Adjust unit count according to policy combat bonuses
      int adjustedUnits = units;
      if (target_region.Policy == policy.industry && Policy != policy.industry) {
        adjustedUnits = Mathf.FloorToInt(units * 0.8f);
      } else if (Policy == policy.industry && target_region.Policy != policy.industry) {
        adjustedUnits = Mathf.FloorToInt(units * 1.2f);
      }

      //if no, check if the target region has more or less units
      if (target_region.units < adjustedUnits) {
        // Trigger sound effect
        if (target_region.Owner == player.none) {
          mixer.playSFX("neutral region");
        } else {
          mixer.playSFX("enemy region");
        }
        // Update ownership and unit count
        target_region.road_Hub.destroy_road();
        target_region.Owner = start_owner;
        target_region.affect_some_nearby_trees();
        target_region.units = Mathf.Min(units, adjustedUnits - target_region.units);
      } else if (target_region.units == adjustedUnits) {//turn the region neutral
        target_region.road_Hub.destroy_road();
        target_region.Owner = player.none;
        target_region.units = 0;
      } else {
        target_region.units -= adjustedUnits;
      }
    }

    Destroy(gameObject);
  }

  // Called once per frame, after update()
  private void LateUpdate() {
    if (start_owner == player.A) {
      pA_has_units = true;
    } else if (start_owner == player.B) {
      pB_has_units = true;
    }
  }

  // Add an additional unit sprite (strictly visual)
  private void addSingleUnitSR() {
    SpriteRenderer newSR = Instantiate(single_sr, transform);
    //newSR.transform.position = single_sr.transform.position;
    float delta_x = Random.Range(-sr_radius, sr_radius);
    float delta_y = Random.Range(-sr_radius, sr_radius);
    newSR.transform.position += new Vector3(delta_x, delta_y);
    newSR.GetComponent<Animator>().SetBool("blue", start_owner == player.B);

    single_unit_sr_total++;
  }
}
