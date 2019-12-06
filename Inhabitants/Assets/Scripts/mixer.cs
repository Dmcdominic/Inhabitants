using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mixer : MonoBehaviour {
  public AudioSource musicPlayer;
  public AudioClip introClip, eco1Clip, eco2Clip, eco3Clip, neuEcoClip, neuIndClip,
      ind1Clip, ind2Clip, ind3Clip, ecoVictory, indVictory, tie, waves;
  public static AudioSource fxSource;
  public float ecoCutoff = 0.45f;
  public float indCutoff = 0.55f;
  public float industryLevel = 0f;
  public static gamestate gS;
  public static winstate wS;
  public bool indPlayed = false;
  public bool gameEnded = true;


  // Init
  private void Awake() {
    fxSource = GameObject.Find("Sound/FX").GetComponent(typeof(AudioSource)) as AudioSource;
  }

  // Start is called before the first frame update
  void Start() {
    musicPlayer.clip = introClip;
    musicPlayer.Play();
  }

  // Update is called once per frame
  void Update() {
    // Update the current industry value
    industryLevel = status_controller.industryLevel;

    // Update the current game state / win state
    // start_to_join, playing, winscreen / empires_falling (eco, ind, tie)
    gS = PlayerManager.Gamestate;
    wS = PlayerManager.Winstate;

    // Play Victory Music (Once)
    if (gameEnded && (gS == gamestate.winscreen || gS == gamestate.empires_falling)) {
      switch (wS) {
        case winstate.eco:
          musicPlayer.Stop();
          musicPlayer.clip = ecoVictory;
          musicPlayer.Play();
          break;
        case winstate.ind:
          musicPlayer.Stop();
          musicPlayer.clip = indVictory;
          musicPlayer.Play();
          break;
        case winstate.tie:
          musicPlayer.Stop();
          musicPlayer.clip = tie;
          musicPlayer.Play();
          break;
        default:
          //musicPlayer.Stop();
          //musicPlayer.clip = ecoVictory;
          //musicPlayer.Play();
          Debug.LogError("Game has not ended");
          break;
      }

      gameEnded = false;
    }

    // Play Intro (Start Screen) Music
    if (!musicPlayer.isPlaying && gS == gamestate.start_to_join) {
      Start();
      gameEnded = true;
    }

    // Play regular music
    if (!musicPlayer.isPlaying && gS == gamestate.playing) {
      if (industryLevel < ecoCutoff) {
        float eco_r = Random.Range(0.0f, 3.0f);
        if (eco_r < 1.0f) musicPlayer.clip = eco1Clip;
        else if (eco_r < 2.0f) musicPlayer.clip = eco2Clip;
        else musicPlayer.clip = eco3Clip;
        indPlayed = false;
      } else if (industryLevel < indCutoff) {
        if (!indPlayed) {
          musicPlayer.clip = neuEcoClip;
          indPlayed = true;
        } else {
          musicPlayer.clip = neuIndClip;
          indPlayed = false;
        }
      } else {
        float ind_r = Random.Range(0.0f, 3.0f);
        if (ind_r < 1.0f) musicPlayer.clip = ind1Clip;
        else if (ind_r < 2.0f) musicPlayer.clip = ind2Clip;
        else musicPlayer.clip = ind3Clip;
        indPlayed = true;
      }
      musicPlayer.Play();
    }

    // Play waves crashing sounds
    if (!musicPlayer.isPlaying && gS == gamestate.sea_levels_rose) {
      musicPlayer.clip = waves;
      musicPlayer.Play();
    }
  }


  public static void playSFX(string sfx) {
    if (!fxSource)
      fxSource = GameObject.Find("Sound/FX").GetComponent(typeof(AudioSource)) as AudioSource;
    AudioClip clip = (AudioClip)Resources.Load<AudioClip>(sfx);

    if (clip)
      fxSource.PlayOneShot(clip);
    else
      Debug.LogError("no sfx associated with " + sfx);
  }

  public static void playSFX(string sfx, int duration) {

    if (!fxSource)
      fxSource = GameObject.Find("Sound/FX").GetComponent(typeof(AudioSource)) as AudioSource;
    AudioSource dupSource = Instantiate(fxSource);
    AudioClip clip = (AudioClip)Resources.Load<AudioClip>(sfx);
    dupSource.clip = clip;

    if (clip) {
      dupSource.PlayOneShot(clip);
      useless mb = dupSource.gameObject.AddComponent(typeof(useless)) as useless;
      mb.StartCoroutine(WaitAndStop(duration, dupSource, mb));
    } else
      Debug.LogError("no sfx associated with " + sfx);
  }

  public static IEnumerator WaitAndStop(float duration, AudioSource source, MonoBehaviour mb) {
    yield return new WaitForSeconds(duration);
    source.Stop();
    Destroy(mb);
    Destroy(source);
  }
}
