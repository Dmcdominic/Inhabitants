using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mixer : MonoBehaviour
{
    public AudioSource musicPlayer;
    public AudioClip introMusic;
    public AudioClip ecoMusic;
    public AudioClip neuMusic;
    public AudioClip indMusic;
    public int ecoCutoff;
    public int indCutoff;
    public int value = 95;


    public static void playSFX(string sfx)
    {
        AudioSource fxSource = GameObject.Find("Sound/FX").GetComponent(typeof(AudioSource)) as AudioSource;
        AudioClip clip = (AudioClip)Resources.Load<AudioClip>(sfx);

        if (clip)
            fxSource.PlayOneShot(clip);
        else
            Debug.LogError("no sfx associated with " + sfx);
    }

    // Start is called before the first frame update
    void Start()
    {
        //musicPlayer = GetComponent<AudioSource>();
        musicPlayer.clip = introMusic;
        musicPlayer.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!musicPlayer.isPlaying)
        {
            if (value < ecoCutoff)
            {
                musicPlayer.clip = ecoMusic;
            }
            else if (ecoCutoff <= value && value < indCutoff)
            {
                musicPlayer.clip = neuMusic;
            }
            else if (indCutoff <= value)
            {
                musicPlayer.clip = indMusic;
            }
            musicPlayer.Play();
        }
    }
}
