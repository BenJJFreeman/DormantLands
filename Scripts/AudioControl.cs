using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioControl : MonoBehaviour
{

    public AudioMixer[] mixer;
    public AudioSource[] source;
    public AudioClip[] music;
    public AudioClip[] ambience;
    public AudioClip[] effect;

    void Start()
    {

        PlayMusic(Random.Range(0, music.Length));
        PlayAmbience(Random.Range(0, ambience.Length));

    }
    void Update()
    {
        
        if(source[0].isPlaying == false)
        {
            PlayMusic(Random.Range(0, music.Length));
        }
        if (source[1].isPlaying == false)
        {
            PlayAmbience(Random.Range(0, ambience.Length));
        }

    }

    public void PlaySoundEffect(int _effect)
    {

        source[2].clip = effect[_effect];
        source[2].Play();
    }
    public void PlayAmbience(int _ambience)
    {
        source[1].clip = ambience[_ambience];
        source[1].Play();
    }
    public void PlayMusic(int _music)
    {
        source[0].clip = music[_music];
        source[0].Play();
    }
}

