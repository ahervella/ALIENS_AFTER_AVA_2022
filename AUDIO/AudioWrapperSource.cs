using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using PowerTools;

public class AudioWrapperSource : MonoBehaviour
{
    [SerializeField]
    private AudioMixerGroup mixerGroup = null;
    public AudioMixerGroup MixerGroup => mixerGroup;


    [SerializeField]
    [Range(0f, 1f)]
    private float spatialBlend = 0;
    public float SpatialBlend => spatialBlend;

    //TODO: do we want / ever need a custom max distance?
    /*
    [SerializeField]
    [Range(0f, 100f)]
    private float maxDist = 100;
    public float MaxDist => maxDist;*/

    [SerializeField]
    private AnimationEventExtender<AAudioWrapperV2> optionalAEExtender = null;

    [SerializeField]
    private int aeMethodIndex = 0;

    private Dictionary<AudioSource, float> ogVolDict = new Dictionary<AudioSource, float>();


    private void Awake()
    {
        if (optionalAEExtender != null)
        {
            optionalAEExtender.AssignAnimationEvent(obj => AE_PlayAudioWrapper(obj), aeMethodIndex);
        }
        //Don't think we need these since we assign the source
        //properties before playing an audio wrapper anyways
        /*
            foreach (AudioSource source in GetComponents<AudioSource>())
            {
                source.outputAudioMixerGroup = mixerGroup;
                source.spatialBlend = spatialBlend;
                source.maxDistance = maxDist;
                source.rolloffMode = AudioRolloffMode.Custom;
            }*/
    }

    //TODO: if later we find the need to fade different playing sounds from the same source
    //or if we want to make sure that any delayed sounds that have a longer delay than
    //the time it takes to fade out are still left at zero once they are played
    //THEN, we need have a system in here that in which calling PlayAudioWrapperAndFade
    //in AAudioWrapperV2 passes around the root AAudioWrapperV2 from which is used in a
    //Dictionary<AAudioWrapperV2, Dictionary<AudioSource, float>> here for fading
    //...
    //That's gonna eat up a lot of time right now and don't think we need it /
    //will run into that problem right now sooo tis a todo!

    public void AddNewAudioToFadeVolumeDict(AudioSource audioSource, float ogVolume)
    {
        if (!ogVolDict.ContainsKey(audioSource))
        {
            ogVolDict.Add(audioSource, ogVolume);
            return;
        }
        ogVolDict[audioSource] = ogVolume;
    }

    public float GetOGVolume(AudioSource audioSource)
    {
        return ogVolDict[audioSource];
    }

    public AudioSource[] GetCachedVolumeSources()
    {
        AudioSource[] sources = new AudioSource[ogVolDict.Count];
        ogVolDict.Keys.CopyTo(sources, 0);
        return sources;
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

    private void OnDestroy()
    {
        S_AudioManager.OnDestroyCurrent?.OnAudioWrapperSourceDestroyed(this);
    }
}