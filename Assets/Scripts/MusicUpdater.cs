using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicUpdater : MonoBehaviour
{
    public AudioClip audioClip1;
    public AudioClip audioClip2;
    public AudioClip audioClip3;
    public AudioClip audioClip4;
    public AudioClip audioClip5;
    public AudioSource audioSource;
    public float bossVolume;
    public float musicValue;
    public float soundValue;

    void Start()
    {
        // Debug.Log("test audio")
        musicValue = PlayerPrefs.GetFloat("musicVolume", 1f);
        audioSource.volume = 0.5f * musicValue;

        soundValue = PlayerPrefs.GetFloat("soundVolume", 1f);
        bossVolume = 0.4f * soundValue;
    }

    public void UpdateMusic(int level)
    {
        if((level % 25) < 6)
        {
            audioSource.clip = audioClip1;
            if(!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            return;
        }
        if((level % 25) < 11)
        {
            audioSource.clip = audioClip2;
            if(!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            return;
        }
        if((level % 25) < 16)
        {
            audioSource.clip = audioClip3;
            if(!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            return;
        }
        if((level % 25) < 21)
        {
            audioSource.clip = audioClip4;
            if(!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            return;
        }
        if((level % 25) < 26)
        {
            audioSource.clip = audioClip5;
            if(!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            return;
        }
    }
}
