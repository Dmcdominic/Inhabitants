using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class moving_units : MonoBehaviour {
  // Static settings
  private static float movespeed = 0.25f;
  private static float sr_radius = 0.2f;

  // Components
  public region target_region;
  public player start_owner;
  public Vector3 start_position;
  public int units;

  public TextMeshPro units_num;
  public SpriteRenderer single_sr;


  // Start is called before the first frame update
  void Start() {
    Vector2 direction = target_region.centerpoint - (Vector2)transform.position;
    if (direction.x < 0) {
      Vector3 lEA = single_sr.transform.localEulerAngles;
      single_sr.transform.localEulerAngles = new Vector3(lEA.x, 0, lEA.z);
    }

    // TODO - remove this recoloring vv
    single_sr.color = player_data.colors[(int)start_owner];

    // Set up a number of redcoat sprites depending on unit count
    // Thresholds increase cubically
    int threshold = 2;
    while (units > threshold*threshold*threshold) {
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

    //moves the units
    if (Vector2.Distance(transform.position, target_region.centerpoint) > ds) {
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
  }
}
