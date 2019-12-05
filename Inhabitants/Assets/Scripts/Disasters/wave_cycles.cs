using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wave_cycles : MonoBehaviour {
  // Constant settings
  private const float offset_interval = 0.7f;

  // Public fields
  public float interval = 1f;
  public float amplitude = 2f;
  public List<GameObject> waves;

  // Private vars
  private List<Vector2> initLocalPos;
  private List<float> offsets;


  // Start is called before the first frame update
  void Start() {
    offsets = new List<float>();
    initLocalPos = new List<Vector2>();

    float nextOffset = 0;
    for (int i=0; i < waves.Count; i++) {
      initLocalPos.Add(waves[i].transform.localPosition);
      offsets.Add(nextOffset += offset_interval);
    }
  }

  // Update is called once per frame
  void Update() {
    for (int i=0; i < waves.Count; i++) {
      float dX = Mathf.Abs(((Time.time + offsets[i]) % interval) - (interval * 0.5f)) - (interval * 0.25f);
      float dY = Mathf.Abs(((Time.time + offsets[i] - (interval * 0.25f)) % interval) - (interval * 0.5f)) - (interval * 0.25f);
      waves[i].transform.localPosition = initLocalPos[i] + new Vector2(dX * amplitude / interval, dY * amplitude / interval);
    }
  }
}
