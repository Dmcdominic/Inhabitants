using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mixer : MonoBehaviour
{
    public AudioSource musicPlayer;
    public AudioClip introClip;
    public AudioClip eco1Clip, eco2Clip, eco3Clip;
	public AudioClip neuEcoClip, neuIndClip;
	public AudioClip ind1Clip, ind2Clip, ind3Clip;
	public static AudioSource fxSource;
    public float eco1Cutoff = 0.15f;
    public float eco2Cutoff = 0.30f;
    public float eco3Cutoff = 0.45f;
    public float ind1Cutoff = 0.55f;
    public float ind2Cutoff = 0.70f;
    public float ind3Cutoff = 0.85f;
    public float industryLevel = 0f;
    private bool indPlayed = false;
    private bool gameStarted = false;

    public static void playSFX(string sfx)
    {
        if (!fxSource)
            fxSource = GameObject.Find("Sound/FX").GetComponent(typeof(AudioSource)) as AudioSource;
        AudioClip clip = (AudioClip)Resources.Load<AudioClip>(sfx);

        if (clip)
            fxSource.PlayOneShot(clip);
        else
            Debug.LogError("no sfx associated with " + sfx);
    }

    public static void playSFX(string sfx, int duration)
    {

        if (!fxSource)
            fxSource = GameObject.Find("Sound/FX").GetComponent(typeof(AudioSource)) as AudioSource;
        AudioSource dupSource = Instantiate(fxSource);
        AudioClip clip = (AudioClip)Resources.Load<AudioClip>(sfx);
        dupSource.clip = clip;
      

        if (clip)
        {
            dupSource.PlayOneShot(clip);
            useless mb = dupSource.gameObject.AddComponent(typeof(useless)) as useless;
            mb.StartCoroutine(WaitAndStop(duration, dupSource, mb));
        }
        else
            Debug.LogError("no sfx associated with " + sfx);



    }

    public static IEnumerator WaitAndStop(float duration, AudioSource source, MonoBehaviour mb)
    {
        yield return new WaitForSeconds(duration);
        source.Stop();
        Destroy(mb);
        Destroy(source);
    }

	private void Awake()
    {
        fxSource = GameObject.Find("Sound/FX").GetComponent(typeof(AudioSource)) as AudioSource;
    }

    // Start is called before the first frame update
    void Start()
    {
        musicPlayer.clip = introClip;
        musicPlayer.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // Update the current industry value
        industryLevel = status_controller.industryLevel;

        if (!musicPlayer.isPlaying)
        {
            if (industryLevel < eco1Cutoff)
            {
                musicPlayer.clip = eco1Clip;
				indPlayed = false;
            }
			else if (eco1Cutoff <= industryLevel && industryLevel < eco2Cutoff)
			{
				musicPlayer.clip = eco2Clip;
				indPlayed = false;
			}
			else if (eco2Cutoff <= industryLevel && industryLevel < eco3Cutoff)
			{
				musicPlayer.clip = eco3Clip;
				indPlayed = false;
			}
			else if (eco3Cutoff <= industryLevel && industryLevel < ind1Cutoff)
			{
				if (!indPlayed)
				{
					musicPlayer.clip = neuEcoClip;
					indPlayed = true;
				}
				else {
					musicPlayer.clip = neuIndClip;
					indPlayed = false;
                }
			}
			else if (ind1Cutoff <= industryLevel && industryLevel < ind2Cutoff)
			{
				musicPlayer.clip = ind1Clip;
				indPlayed = true;
			}
			else if (ind2Cutoff <= industryLevel && industryLevel < ind3Cutoff)
			{
				musicPlayer.clip = ind2Clip;
				indPlayed = true;
			}
			else if (ind3Cutoff <= industryLevel)
            {
                musicPlayer.clip = ind3Clip;
				indPlayed = true;
			}
            musicPlayer.Play();
        }
    }
}
