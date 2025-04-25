using System;
using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{

    [SerializeField] private AudioMixer audioMixer;
   
    public void SetMusicVolume(float level)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
    }

    public void SetSFXVolume(float level)
    {
        
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(level) * 20f);
    }
}
