using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReticleControlInput;
using XboxCtrlrInput;

public enum policy { none, eco, neutral, industry };

public class policy_manager : MonoBehaviour {

  public static policy[] policies = new policy[] { policy.none, policy.neutral, policy.neutral };

  public static int policiesCount = System.Enum.GetNames(typeof(policy)).Length;

  public List<SpriteRenderer> p1PolicyDisplays;
  public List<SpriteRenderer> p2PolicyDisplays;


  // Update is called once per frame
  void Update() {
    if (PlayerManager.Gamestate == gamestate.sea_levels_rose) {
      return;
    }

    // Take in player input for switching policies
    for (int i = 0; i < 2; i++) {
      int Player = (i == 0) ? (int)player.A : (int)player.B;
      int currentPolicy = (int)policies[Player];
      policy newPolicy = (policy)currentPolicy;

      XboxController controller = player_data.controllers[Player];
      if (RCI.GetButtonDown(XboxButton.DPadDown, controller)) {
        newPolicy = (policy)Mathf.Clamp(currentPolicy - 1, 1, policiesCount - 1);
      } else if (RCI.GetButtonDown(XboxButton.DPadUp, controller)) {
        newPolicy = (policy)Mathf.Clamp(currentPolicy + 1, 1, policiesCount - 1);
      }

      ChangePolicyBonusDisplay(i, currentPolicy, newPolicy);

      if (newPolicy != policies[Player]) {
        policies[Player] = newPolicy;
        //city.updatePoliciesAll(Player, (policy)newPolicy);
        // Play corresponding sound effect
        if (newPolicy == policy.industry) {
          mixer.playSFX("ind policy");
        } else if (newPolicy == policy.neutral) {
          mixer.playSFX("population growth");
        } else if (newPolicy == policy.eco) {
          mixer.playSFX("eco policy");
        }
      }
    }

    // Update policy displays
    for (int pol = 0; pol < policiesCount; pol++) {
      if (p1PolicyDisplays[pol] != null) {
        setPolicySR(p1PolicyDisplays[pol], policies[1] == (policy)pol);
      }
      if (p2PolicyDisplays[pol] != null) {
        setPolicySR(p2PolicyDisplays[pol], policies[2] == (policy)pol);
      }
    }
  }

  // Sets an sr to be faded if its policy is non-active, or fully-colored if it is
  private void setPolicySR(SpriteRenderer sr, bool active) {
    if (active) {
      sr.color = Color.white;
    } else {
      sr.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }
  }

  // Activates a new bonus display and deactivates the previous one
  private void ChangePolicyBonusDisplay(int playerNum, int oldPolicy, policy newPolicy) {
    transform.GetChild(playerNum).GetChild(Mathf.Abs((int)oldPolicy - 4)).GetChild(0).gameObject.SetActive(false);
    transform.GetChild(playerNum).GetChild(Mathf.Abs((int)newPolicy - 4)).GetChild(0).gameObject.SetActive(true);
  }
}
