using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;
using TMPro;

public class AudioDataDisplay : DataDisplay
{
    [HideInInspector]
    public string audioname;

    AudioSource audioSource;

    string datapath;

    public override void Awake() {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }

    public override void ToggleDataOn()
    {
        base.ToggleDataOn();

        // Getting audio clip matches the recorded audio name
        if(AudioRecordTracker.audiotracker.audioclips.Count != 0)
        {
            foreach(AudioClip audio in AudioRecordTracker.audiotracker.audioclips)
            {
                if(audio.name == audioname)
                {
                    audioSource.clip = audio;
                }
            }
        }
    }

    public void PlayAudio()
    {
        if(!audioSource.isPlaying)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Pause();
        }
    }
}
