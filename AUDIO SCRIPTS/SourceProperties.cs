using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SourceProperties : MonoBehaviour
{
    public AudioMixerGroup output;

    [Range(0f, 1f)]
    public float spatialBlend = 0;

    [Range(0f, 100f)]
    public float maxDist = 100;

    private void Awake()
    {
        foreach (AudioSource source in GetComponents<AudioSource>())
        {
            source.outputAudioMixerGroup = output;
            source.spatialBlend = spatialBlend;
            source.maxDistance = maxDist;
            source.rolloffMode = AudioRolloffMode.Custom;
        }
    }
}
