using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager> {

    public float highPitchRange = 1.2f;
    public float lowPitchRange = 0.7f;
    private AudioSource MusicSource;

    private void Awake()
    {
        MusicSource = GetComponent<AudioSource>();
        base.Awake();
    }

    /// <summary>
    /// Set background music.
    /// </summary>
    public void SetMusic(AudioClip new_clip)
    {
        MusicSource.clip = new_clip;
        MusicSource.Play();
    }

    //Used to play single sound clips.
    public void PlaySingle(AudioClip clip, AudioSource source)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        source.clip = clip;

        //Play the clip.
        source.Play();
    }


    //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
    //It plays it using the passed in AudioSource of the one making the sound.
    public void RandomizeSfx(AudioClip clip, AudioSource source)
    {
        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        //Set the pitch of the audio source to the randomly chosen pitch.
        source.pitch = randomPitch;

        //Set the clip to the clip at our randomly chosen index.
        source.clip = clip;
        //Play the clip.
        source.Play();
    }
}
