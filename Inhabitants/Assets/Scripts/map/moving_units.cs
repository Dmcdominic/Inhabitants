using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class moving_units : MonoBehaviour {
  // Static settings
  private const float movespeed = 0.25f;
  private const float sr_radius = 0.35f;
  private const float arrival_radius = 0.25f;

  private const float tree_destr_odds = 0.15f;
  private const float tree_destr_radius = 0.35f;
  private const float tree_destr_delta = -0.35f;

  // Components
  public region target_region;
  public player start_owner;
  public Vector3 start_position;
  public int units;

  public TextMeshPro units_num;
  public SpriteRenderer single_sr;


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
  }

  // Update is called once per frame
  void Update() {
    float ds = Time.deltaTime * movespeed;

    units_num.text = units.ToString();
    units_num.color = player_data.colors[(int)start_owner];

    //checks if the # of units moving has gone down to zero. Terminate if so.
    if (units <= 0) {
      Destroy(this.gameObject);
      return;
    }

    // Small odds to destroy nearby trees
    if (Random.Range(0, 1f) < tree_destr_odds * Time.deltaTime) {
      cell_controller.instance.growTrees(transform.position, tree_destr_radius, tree_destr_delta);
    }

    // If we're not close enough, move the units
    float dist = Vector2.Distance(transform.position, target_region.centerpoint);
    if (dist > ds && dist > arrival_radius) {
      Vector2 direction = target_region.centerpoint - (Vector2)transform.position;
      transform.Translate(direction.normalized * ds);
      return;
    }

    //check if the target region is friendly, if yes
    if (target_region.Owner == start_owner) {
      target_region.units += units;
    } else {
      //if no, check if the target region has more or less units
      if (target_region.units < units) {
        target_region.road_Hub.destroy_road();
        target_region.Owner = start_owner;
        target_region.clear_some_nearby_trees();
        target_region.units = units - target_region.units;
      } else if (target_region.units == units) {//turn the region neutral
        target_region.road_Hub.destroy_road();
        target_region.Owner = player.none;
        target_region.units = 0;
      } else {
        target_region.units -= units;
      }
    }

    Destroy(this.gameObject);
  }

  // Add an additional unit sprite (strictly visual)
  private void addSingleUnitSR() {
    SpriteRenderer newSR = Instantiate(single_sr, transform);
    //newSR.transform.position = single_sr.transform.position;
    float delta_x = Random.Range(-sr_radius, sr_radius);
    float delta_y = Random.Range(-sr_radius, sr_radius);
    newSR.transform.position += new Vector3(delta_x, delta_y);
    newSR.GetComponent<Animator>().SetBool("blue", start_owner == player.B);
  }
}
