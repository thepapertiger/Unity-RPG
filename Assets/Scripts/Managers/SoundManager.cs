using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager> {

    private AudioSource Music;

    private void Awake()
    {
        Music = GetComponent<AudioSource>();
        base.Awake();
    }

    public void SetMusic(AudioClip new_clip)
    {
        Music.clip = new_clip;
        Music.Play();
    }
}
