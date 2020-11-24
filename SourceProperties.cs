using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SourceProperties : MonoBehaviour
{
    public AudioMixerGroup output;

    void Awake()
    {
        foreach (AudioSource source in GetComponents<AudioSource>())
        {
            source.outputAudioMixerGroup = output;
        }
    }
}
