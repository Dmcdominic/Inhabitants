using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deer : MonoBehaviour {
  public deer_reps deer_rep;
  public float colony_num = 100f;
  private const float max_colony_num = 100f;
  private const float min_colony_num = max_colony_num / 10f;
  public float tree_effect = 1f;
  private CircleCollider2D cc;
  public float radius = 1f;
  private List<deer_reps> deer_r = new List<deer_reps>();

  private float tree_d;


  // Start is called before the first frame update
  void Start() {
    cc = GetComponent<CircleCollider2D>();
    gameObject.name = "deer";
    for (int i = 0; i < 5; i++) {
      Vector3 pos = Random.insideUnitCircle * radius;
      deer_reps r = Instantiate(deer_rep, transform.position + pos, Quaternion.identity);
      r.radius = radius;
      r.transform.parent = transform;
      r.basestartpoint = r.transform.position;
      deer_r.Add(r);
    }

  }

  // Update is called once per frame
  void Update() {
    //parameters
    float tree_decrease_rate = -Time.deltaTime * colony_num * 0.001f;
    float tree_spread_rate = Time.deltaTime * colony_num;
    float colony_decrease_rate = 10f * Time.deltaTime;
    float wolf_kill_rate = 10f * Time.deltaTime;

    // Spread trees within the radius
    //cell_controller.instance.growTrees(transform.position, radius, tree_decrease_rate);
    cell_controller.instance.spread_trees(transform.position, radius, tree_spread_rate);

    // Adjust colony size based on tree density
    float deer_d = colony_num / max_colony_num;
    // TODO - figure out how to normalize this. /20f is a fudge number...
    if (tree_d == 0 || Random.Range(0, 3) == 0) {
      tree_d = cell_controller.instance.tree_density(transform.position, radius) / 17f;
    }
    //Debug.Log("tree_d: " + tree_d);
    //Debug.Log($"deer_d: {deer_d}");
    colony_num += (tree_d - deer_d) * colony_decrease_rate;

    //check if there are wolves nearby; if so, decrease size of colony 
    Collider2D[] nearObjects = Physics2D.OverlapCircleAll(transform.position, radius);
    foreach (Collider2D obj in nearObjects) {
      if (obj != null && obj.name == "wolf") {
        colony_num -= wolf_kill_rate;
      }
    }

    // Enable/disable deer representations
    float per = colony_num / max_colony_num;
    for (int i = 1; i < deer_r.Count; i++) {
      deer_r[i].transform.gameObject.SetActive(per > 0.2f * i);
    }


    //if there are no deers left
    if (colony_num <= min_colony_num) {
      Destroy(gameObject);
    }
  }

}
