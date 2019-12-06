using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deer : MonoBehaviour {
  public deer_reps deer_rep;
  public float colony_num = 45f;
  public float tree_effect = 1f;

  private const float max_colony_num = 100f;
  private const float min_colony_num = max_colony_num / 4f;
  private const float colony_adjust_rate = 15f;
  private const float base_growth = 1.3f;

  private CircleCollider2D cc;
  public float radius = 1f;
  private List<deer_reps> deer_r = new List<deer_reps>();

  private float tree_d;

  // A queue to delay colony effects
  private float baby_colony_num;
  private Queue<float> delayed_colony = new Queue<float>();
  private int frame_delay = 110;



  // Start is called before the first frame update
  void Start() {
    cc = GetComponent<CircleCollider2D>();
    gameObject.name = "deer";

    baby_colony_num = colony_num;
    tree_d = cell_controller.instance.tree_density(transform.position, radius);

    for (int i = 0; i < 5; i++) {
      Vector3 pos = Random.insideUnitCircle * radius * 0.7f;
      deer_reps r = Instantiate(deer_rep, transform.position + pos, Quaternion.identity);
      r.radius = radius * 0.7f;
      r.transform.parent = transform;
      r.basestartpoint = r.transform.position;
      deer_r.Add(r);
    }

    // Play sound effect
    mixer.playSFX("deer");
  }

  // Update is called once per frame
  void Update() {
    //parameters
    float tree_decrease_rate = -Time.deltaTime * colony_num * 0.001f;
    float tree_spread_rate = Time.deltaTime * baby_colony_num * 0.8f;
    float wolf_kill_rate = 2f * Time.deltaTime;

    // Spread trees within the radius
    cell_controller.instance.growTrees(transform.position, radius, tree_decrease_rate);
    cell_controller.instance.spread_trees(transform.position, radius, tree_spread_rate);

    // Adjust colony size based on tree density
    float deer_d = colony_num / max_colony_num;
    
    if (Random.Range(0, 3) == 0) {
      tree_d = cell_controller.instance.tree_density(transform.position, radius);
    }

    baby_colony_num += ((tree_d - deer_d) * colony_adjust_rate + base_growth) * Time.deltaTime;
    delayed_colony.Enqueue(baby_colony_num);

    if (delayed_colony.Count >= frame_delay) {
      colony_num = delayed_colony.Dequeue();
    }
    
    
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

  public void hurt_deer(float fraction_killed = 0.2f) {
    colony_num -= colony_num * fraction_killed;
  }
}
