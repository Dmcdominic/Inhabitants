using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class city : MonoBehaviour {

  // Public fields
  public SpriteRenderer sr;
  public Sprite industrySprite;
  public Sprite neutralSprite;
  public Sprite ecoSprite;
  public Sprite untakenSprite;

  // Hidden vars
  [HideInInspector]
  public region Region;
  [HideInInspector]
  public policy Policy;

  // Getters
  private int Owner {
    get { return (int)Region.Owner; }
  }


  // Initialization
  private void Start() {
    setPolicy(policy_manager.policies[Owner]);
  }

  // Check for policy changes
  private void Update() {
    if (policy_manager.policies[Owner] != Policy) {
      setPolicy(policy_manager.policies[Owner]);
    }
  }

  // Set this city to a new policy
  public void setPolicy(policy newPolicy) {
    Policy = newPolicy;
    switch (newPolicy) {
      case policy.industry:
        sr.sprite = industrySprite;
        Region.affect_some_nearby_trees();
        break;
      case policy.neutral:
        sr.sprite = neutralSprite;
        break;
      case policy.eco:
        sr.sprite = ecoSprite;
        Region.affect_some_nearby_trees(0.9f, 0.4f);
        break;
      case policy.none:
        sr.sprite = untakenSprite;
        break;
    }
  }
}
