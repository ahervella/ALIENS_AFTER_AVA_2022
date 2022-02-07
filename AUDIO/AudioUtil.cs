using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioUtil
{
    /// <summary>
    /// Assigns some commonly used source properties to the given object,
    /// omits information that could be given by a SourceProperties script on the same GameObject as the target AudioSource
    /// </summary>
    /// <param name="source">AudioSource to be modified</param>
    /// <param name="volDb">Volume to be assigned in decibels</param>
    /// <param name="pitchCents">Pitch to be assigned in cents</param>
    /// <param name="clip">AudioClip to be assigned</param>
    public static void AssignSourceProperties(AudioSource source, float volDb, float pitchCents, AudioClip clip)
    {
        SourceProperties sp = source.gameObject.GetComponent<SourceProperties>();
        AssignSourceProperties(source, sp.output, sp.spatialBlend, sp.maxDist, volDb, pitchCents, clip);
    }

    /// <summary>
    /// Assigns some commonly used source properties to the given object,
    /// omits information that could be given by a SourceProperties script on the same GameObject as the target AudioSource
    /// </summary>
    /// <param name="source">AudioSource to be modified</param>
    /// <param name="output">AudioMixerGroup to be assigned</param>
    /// <param name="volDb">Volume to be assigned in decibels</param>
    /// <param name="pitchCents">Pitch to be assigned in cents</param>
    /// <param name="clip">AudioClip to be assigned</param>
    public static void AssignSourceProperties(AudioSource source, AudioMixerGroup output, float volDb, float pitchCents, AudioClip clip)
    {
        if (output == null)
        {
            AssignSourceProperties(source, volDb, pitchCents, clip);
            return;
        }
        SourceProperties sp = source.gameObject.GetComponent<SourceProperties>();
        AssignSourceProperties(source, output, sp.spatialBlend, sp.maxDist, volDb, pitchCents, clip);
    }


    /// <summary>
    /// Assigns some commonly used source properties to the given object
    /// </summary>
    /// <param name="source">AudioSource to be modified</param>
    /// <param name="output">AudioMixerGroup to be assigned</param>
    /// <param name="spatialBlend">Spatial blend value to be assigned</param>
    /// <param name="maxDist">Maximum Distance to be assigned</param>
    /// <param name="volDb">Volume to be assigned in decibels</param>
    /// <param name="pitchCents">Pitch to be assigned in cents</param>
    /// <param name="clip">AudioClip to be assigned</param>
    public static void AssignSourceProperties(AudioSource source, AudioMixerGroup output, float spatialBlend, float maxDist, float volDb, float pitchCents, AudioClip clip)
    {
        source.outputAudioMixerGroup = output;
        source.spatialBlend = spatialBlend;
        source.maxDistance = maxDist;
        source.volume = Mathf.Pow(10, volDb / 20);
        source.pitch = Mathf.Pow(2, pitchCents / 1200);
        source.rolloffMode = AudioRolloffMode.Custom;
        source.clip = clip;
    }


    /// <summary>
    /// Gets an audio source to use on the specified GameObject, or creates a new one if none is available
    /// </summary>
    /// <param name="obj">GameObject to get and AudioSource From</param>
    /// <returns>An available or new AudioSource</returns>
    public static AudioSource GetAudioSource(GameObject obj)
    {
        AudioSource[] audioSources = obj.GetComponents<AudioSource>();

        if (audioSources != null)
        {
            for (int i = 0; i < audioSources.Length; i++) //Checks for available audio sources
            {
                AudioSource thisSource = audioSources[i];

                if (!thisSource.isPlaying)//If it isn't playing
                {
                    return thisSource; //Use this source
                }
            }
        }
        //Debug.Log("<<" + obj.name + ">> required an additional audio source");
        AudioSource addedSource = obj.AddComponent<AudioSource>();
        return addedSource;
    }
}
