using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wolf : MonoBehaviour {
  public wolf_reps wolf_rep;
  public float colony_num = 50f;
  private float max_colony_num = 50f;
  private CircleCollider2D cc;
  public float radius = 1f;
  private List<wolf_reps> wolf_r = new List<wolf_reps>();


  // Start is called before the first frame update
  void Start() {
    cc = GetComponent<CircleCollider2D>();
    gameObject.name = "wolf";
    for (int i = 0; i < 5; i++) {
      Vector3 pos = Random.insideUnitCircle * radius;
      wolf_reps r = Instantiate(wolf_rep, transform.position + pos, Quaternion.identity);
      r.radius = radius;
      r.transform.parent = transform;
      r.basestartpoint = transform.position;
      wolf_r.Add(r);
    }

    // Play sound effect
    mixer.playSFX("wolf");
  }

  // Update is called once per frame
  void Update() {
    //parameters
    float colony_decrease_rate = 10f * Time.deltaTime;
    float colony_increase_rate = 4f * Time.deltaTime;
    bool isDeer = false;

    //checks if there are deers nearby
    Collider2D[] nearObjects = Physics2D.OverlapCircleAll(transform.position, radius);
    foreach (Collider2D obj in nearObjects) {
      if (obj != null && obj.name == "deer") {
        isDeer = true;
      }
    }

    //if there is no deer nearby, decrease colony size
    if (!isDeer) {
      colony_num -= colony_decrease_rate;
    } else {
      colony_num += colony_increase_rate;
    }

    // Enable/disable wolf representations
    float per = colony_num / max_colony_num;
    for (int i = 1; i < wolf_r.Count; i++) {
      wolf_r[i].transform.gameObject.SetActive(per > 0.2f * i);
    }

    //if there are no wolves left
    if (colony_num <= 0) {
      Destroy(gameObject);
    }
  }

}
