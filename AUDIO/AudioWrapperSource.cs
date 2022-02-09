using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioWrapperSource : MonoBehaviour
{
    [SerializeField]
    private AudioMixerGroup mixerGroup = null;
    public AudioMixerGroup MixerGroup => mixerGroup;


    [SerializeField]
    [Range(0f, 1f)]
    private float spatialBlend = 0;
    public float SpatialBlend => spatialBlend;

    [SerializeField]
    [Range(0f, 100f)]
    private float maxDist = 100;
    public float MaxDist => maxDist;


    private void Awake()
    {
        foreach (AudioSource source in GetComponents<AudioSource>())
        {
            source.outputAudioMixerGroup = mixerGroup;
            source.spatialBlend = spatialBlend;
            source.maxDistance = maxDist;
            source.rolloffMode = AudioRolloffMode.Custom;
        }
    }

    /// <summary>
    /// Public method to be called by this object's animation events
    /// </summary>
    /// <param name="wrapper"></param>
    public void AE_PlayAudioWrapper(AAudioWrapperV2 wrapper)
    {
        wrapper.PlayAudioWrapper(this);
    }

    public void SetMixerGroup(AudioMixerGroup mixerGroup)
    {
        this.mixerGroup = mixerGroup;
    }
}