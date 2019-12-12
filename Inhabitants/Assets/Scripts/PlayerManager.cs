using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using XboxCtrlrInput;
using ReticleControlInput;

public enum gamestate { start_to_join, playing, winscreen, empires_falling, sea_levels_rose };
public enum winstate { none, eco, ind, tie };

public class PlayerManager : MonoBehaviour {
  // Static settings
#if UNITY_EDITOR
  public const float human_playtime = 10f;
#else
  public const float human_playtime = 420f;
#endif
  public const float winscreen_time = 8f;

  // Public fields
  public Canvas playerCanvas;
  public GameObject p1Reticle;
  public GameObject p2Reticle;
  public region p1StartArea;    // 0
  public region p2StartArea;    // 16

  public GameObject p1JoinText;
  public GameObject p2JoinText;
  public TextMeshProUGUI timer_TMP;
  public TextMeshProUGUI winner_TMP;
  public TextMeshProUGUI empires_fall_TMP;
  public TextMeshProUGUI sea_levels_rose_TMP;

  public static gamestate Gamestate;
  public static winstate Winstate;

  // Private vars
  private float timer = human_playtime;
  private float sea_level_timer;


  // Start is called before the first frame update
  void Start() {
    init();
  }

  // Initialize timer and human players game status
  private void init() {
    Gamestate = gamestate.start_to_join;
    Winstate = winstate.none;
    p1Reticle.SetActive(false);
    p2Reticle.SetActive(false);
    p1JoinText.SetActive(true);
    p2JoinText.SetActive(true);
    timer = human_playtime;

    timer_TMP.gameObject.SetActive(false);
    winner_TMP.gameObject.SetActive(false);
    empires_fall_TMP.gameObject.SetActive(false);
    sea_levels_rose_TMP.gameObject.SetActive(false);

    // Init region unit count
    foreach (region Region in region.allRegions) {
      Region.units = region.base_units;
    }
  }

  // Update is called once per frame
  void Update() {
    // START_TO_JOIN
    if (Gamestate == gamestate.start_to_join) {
      // Check for players hitting start to join
      if (RCI.GetButton(XboxButton.Start, XboxController.First)) {
        p1Reticle.SetActive(true);
        p1JoinText.SetActive(false);
      }

      if (RCI.GetButton(XboxButton.Start, XboxController.Second)) {
        p2Reticle.SetActive(true);
        p2JoinText.SetActive(false);
      }

      // Check if for state change
      if (p1Reticle.activeSelf && p2Reticle.activeSelf) {
        Gamestate = gamestate.playing;
        timer_TMP.gameObject.SetActive(true);
        p1StartArea.Owner = player.A;
        p2StartArea.Owner = player.B;
        update_timer_text();
      }
      return;
    }

    // PLAYING
    if (Gamestate == gamestate.playing) {
      // Check for one player eliminated
      bool pA_elim = true;
      bool pB_elim = true;
      foreach (region Region in region.allRegions) {
        pA_elim = (pA_elim && Region.Owner != player.A);
        pB_elim = (pB_elim && Region.Owner != player.B);
        if (!pA_elim && !pB_elim) {
          break;
        }
      }

      pA_elim = pA_elim && !moving_units.pA_has_units;
      pB_elim = pB_elim && !moving_units.pB_has_units;
      if (pA_elim || pB_elim) {
        // Pull up win-screen
        Gamestate = gamestate.winscreen;
        timer_TMP.gameObject.SetActive(false);
        winner_TMP.gameObject.SetActive(true);

        if (pA_elim ? (policy_manager.policies[(int)player.B] == policy.industry)
              : (policy_manager.policies[(int)player.A] == policy.industry)) {
          Winstate = winstate.ind;
        } else {
          Winstate = winstate.eco;
        }

        string empire_won = pA_elim ? "Blue" : "Red";
        winner_TMP.text = "The " + empire_won + "  Empire  is  victorious";
        timer = winscreen_time;
        return;
      }

      moving_units.pA_has_units = false;
      moving_units.pB_has_units = false;

      // Otherwise, iterate the timer
      timer -= Time.deltaTime;
      if (timer > 0) {
        update_timer_text();
      } else {
        // Pull up tie screen
        Gamestate = gamestate.winscreen;
        Winstate = winstate.tie;
        timer_TMP.gameObject.SetActive(false);
        winner_TMP.gameObject.SetActive(true);
        winner_TMP.text = "Both empires lived on...";
        timer = winscreen_time;
      }
      return;
    }

    // WINSCREEN
    if (Gamestate == gamestate.winscreen) {
      timer -= Time.deltaTime;
      if (timer > 0) {
        // This is fine. Waiting for delay to end
      } else if (status_controller.instance.airLevel < 0.1 || status_controller.instance.temperatureLevel < 0.1) {
        // Sea levels rose  
        Gamestate = gamestate.sea_levels_rose;
        winner_TMP.gameObject.SetActive(false);
        sea_levels_rose_TMP.gameObject.SetActive(true);
        sea_level_timer = human_playtime;
        // Set TMP text based on if it's a tie, or one empire
        if (Winstate == winstate.tie) {
          sea_levels_rose_TMP.text = "But as sea levels rose, so each empire fell...";
        } else {
          sea_levels_rose_TMP.text = "But as sea levels rose, so the empire fell...";
        }
      } else {
        // Empires fall
        Gamestate = gamestate.empires_falling;
        winner_TMP.gameObject.SetActive(false);
        empires_fall_TMP.gameObject.SetActive(true);
      }
      return;
    }

    // EMPIRES FALLING
    if (Gamestate == gamestate.empires_falling) {
      // Wait for all regions to be neutral.
      foreach (region Region in region.allRegions) {
        if (Region.Owner != player.none) {
          return;
        }
      }

      // Once it's over:
      init();
      return;
    }

    // SEA LEVELS ROSE
    if (Gamestate == gamestate.sea_levels_rose) {
      sea_level_timer -= Time.deltaTime;
      if (sea_level_timer > 0) {
        // This is fine
        if (sea_level_controller.idle) {
          sea_levels_rose_TMP.text = "...and there was no land left to fight for";
        }
      } else if (Random.Range(0, 2) == 0) {
        // Some chance for the sea levels to stay
        sea_level_timer = 30;
      } else {
        // When this state ends
        init();
        return;
      }
    }
  }

  // Updates the timer text display 
  private void update_timer_text() {
    int minutes = Mathf.FloorToInt(timer / 60f);
    int seconds = Mathf.FloorToInt(timer % 60f);

    string S_minutes = minutes.ToString();
    string S_seconds = seconds.ToString();
    if (S_seconds.Length < 2) {
      S_seconds = "0" + S_seconds;
    }

    timer_TMP.text = S_minutes + ":" + S_seconds;
  }

  // Returns true iff empires are getting cleared
  public static bool empires_clearing {
    get {
      return (Gamestate == gamestate.empires_falling || Gamestate == gamestate.sea_levels_rose);
    }
  }
}
