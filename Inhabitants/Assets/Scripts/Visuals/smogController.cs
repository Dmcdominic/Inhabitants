using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smogController : MonoBehaviour {

  // Static settings
  private const float maxAirLevel = 0.5f;
  private const float initAlphaMult = 1.3f;

  // Public fields
  public List<SpriteRenderer> smog_srs;

  // Private vars
  private List<float> init_alphas;

  private float seaLevelTimer;

  
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
    float seaLevelFactor = 1f;
    if (PlayerManager.Gamestate == gamestate.sea_levels_rose) {
      seaLevelTimer += Time.deltaTime;
      seaLevelFactor = Mathf.Lerp(0.3f, 1f, (5f - seaLevelTimer) / 5f);
    } else {
      seaLevelTimer = 0;
    }

    // Main smog alpha value calculation
    float smogVal = Mathf.Clamp(status_controller.instance.airLevel, 0f, maxAirLevel);
    smogVal = (maxAirLevel - smogVal) * initAlphaMult / (maxAirLevel);

    for (int i = 0; i < smog_srs.Count; i++) {
      color_util.set_alpha(smog_srs[i], init_alphas[i] * smogVal * seaLevelFactor);
    }
  }
}
