using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deer_reps : MonoBehaviour {
  public float radius;

  public Vector2 basestartpoint;
  public Vector2 target;
  public float speed = 0.00001f;

  private const float pause_time_min = 0.3f;
  private const float pause_time_max = 5.5f;

  private float pause_timer = 0f;


  // Start is called before the first frame update
  void Start() {
    target = NextDestination();
  }

  // Update is called once per frame
  void Update() {
    if (pause_timer > 0) {
      pause_timer -= Time.deltaTime;
      return;
    }
    
    float ds = Time.deltaTime * speed;
    if (Vector2.Distance(transform.position, target) > ds) {
      Vector2 direction = target - (Vector2)transform.position;
      transform.Translate(direction.normalized * ds);
      return;
    }
    target = NextDestination();
    pause_timer = Random.Range(pause_time_min, pause_time_max);
  }

  Vector2 NextDestination() {
    return basestartpoint + (Random.insideUnitCircle * radius);
  }
}
