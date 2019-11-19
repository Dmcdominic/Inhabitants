using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smogController : MonoBehaviour {

  // Static settings
  private const float minAirLevel = 0.5f;
  private const float initAlphaMult = 1.3f;

  // Public fields
  public List<SpriteRenderer> smog_srs;

  // Private vars
  private List<float> init_alphas;

  
  // Init
  private void Awake() {
    init_alphas = new List<float>(smog_srs.Count);
    for (int i=0; i < smog_srs.Count; i++) {
      init_alphas.Add(smog_srs[i].color.a);
      color_util.set_alpha(smog_srs[i], 0);
      //smog_srs[i].gameObject.SetActive(true);
      smog_srs[i].transform.parent.gameObject.SetActive(true);
    }
  }

  // Update is called once per frame
  void Update() {
    float smogVal = Mathf.Clamp(status_controller.instance.airLevel, minAirLevel, 1f);
    smogVal = (smogVal - minAirLevel) * initAlphaMult / (1f - minAirLevel);
    for (int i = 0; i < smog_srs.Count; i++) {
      color_util.set_alpha(smog_srs[i], init_alphas[i] * smogVal);
    }
  }
}
