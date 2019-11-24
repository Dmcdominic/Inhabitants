﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using XboxCtrlrInput;
using ReticleControlInput;

public enum gamestate { start_to_join, playing, winscreen, empires_falling };

public class PlayerManager : MonoBehaviour {
  // Static settings
#if UNITY_EDITOR
  public const float human_playtime = 60f;
#else
  public const float human_playtime = 300f;
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

  public static gamestate Gamestate;

  // Private vars
  private float timer = human_playtime;


  // Start is called before the first frame update
  void Start() {
    init();
  }

  // Initialize timer and human players game status
  private void init() {
    Gamestate = gamestate.start_to_join;
    p1Reticle.SetActive(false);
    p2Reticle.SetActive(false);
    p1JoinText.SetActive(true);
    p2JoinText.SetActive(true);
    timer = human_playtime;

    timer_TMP.gameObject.SetActive(false);
    winner_TMP.gameObject.SetActive(false);
    empires_fall_TMP.gameObject.SetActive(false);

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
      if (pA_elim || pB_elim) {
        // Pull up win-screen
        Gamestate = gamestate.winscreen;
        timer_TMP.gameObject.SetActive(false);
        winner_TMP.gameObject.SetActive(true);
        string empire_won = pA_elim ? "Blue" : "Red";
        winner_TMP.text = "The " + empire_won + "  Empire  is  victorious";
        timer = winscreen_time;
        return;
      }

      // Otherwise, iterate the timer
      timer -= Time.deltaTime;
      if (timer > 0) {
        update_timer_text();
      } else {
        // Pull up tie screen
        Gamestate = gamestate.winscreen;
        timer_TMP.gameObject.SetActive(false);
        winner_TMP.gameObject.SetActive(true);
        winner_TMP.text = "Both empires live on...";
        timer = winscreen_time;
      }
      return;
    }

    // WINSCREEN
    if (Gamestate == gamestate.winscreen) {
      timer -= Time.deltaTime;
      if (timer > 0) {
        // This is fine
      } else {
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
      empires_fall_TMP.gameObject.SetActive(false);
      init();
      return;
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
}
