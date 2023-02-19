using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    [SerializeField]
    List<AudioSource> sounds;

    public void MusicControl(bool isOn)
    {
        if (isOn)
            GetComponent<AudioSource>().Play();
        else
            GetComponent<AudioSource>().Stop();
    }

    public void SoundControl(bool isOn)
    {
        if (isOn)
        {
            foreach (AudioSource audio in sounds)
                audio.mute = false;
        }
        else
        {
            foreach (AudioSource audio in sounds)
                audio.mute = true;
        }
    }
  
    public void SetMusicVolume(float volume)
    {
        GetComponent<AudioSource>().volume = volume;
    }

    public void SetSoundVolume(float volume)
    {
        foreach (AudioSource audio in sounds)
            audio.volume = volume;
    }
}
