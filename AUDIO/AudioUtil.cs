using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//TODO: move all audio util stuff to just live in AudioWrapperSource
//The reason for making a util was a legacy concept
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
    public static AudioSource AssignWrapperSourceProperties(AudioWrapperSource aws, float volDb, float pitchCents, AudioClip clip)
    {
        AudioSource source = GetNewAudioSource(aws);
        AssignSourceProperties(source, aws.MixerGroup, aws.SpatialBlend, /*aws.MaxDist,*/ volDb, pitchCents, clip);
        aws.AddNewAudioToFadeVolumeDict(source, source.volume);
        return source;
    }


    /// <summary>
    /// Assigns some commonly used source properties to the given object
    /// </summary>
    /// <param name="source">AudioSource to be modified</param>
    /// <param name="mixerGroup">AudioMixerGroup to be assigned</param>
    /// <param name="spatialBlend">Spatial blend value to be assigned</param>
    /// <param name="maxDist">Maximum Distance to be assigned</param>
    /// <param name="volDb">Volume to be assigned in decibels</param>
    /// <param name="pitchCents">Pitch to be assigned in cents</param>
    /// <param name="clip">AudioClip to be assigned</param>
    private static void AssignSourceProperties(AudioSource source, AudioMixerGroup mixerGroup, float spatialBlend, /*float maxDist,*/ float volDb, float pitchCents, AudioClip clip)
    {
        source.outputAudioMixerGroup = mixerGroup;
        source.spatialBlend = spatialBlend;
        source.maxDistance = S_AudioManager.Current.CachedAudioDist;//maxDist;
        source.volume = Mathf.Pow(10, volDb / 20);
        source.pitch = Mathf.Pow(2, pitchCents / 1200);
        source.rolloffMode = AudioRolloffMode.Custom;
        source.clip = clip;
    }


    /// <summary>
    /// Gets an audio source to use on the specified GameObject, or creates a new one if none is available
    /// </summary>
    /// <param name="source">GameObject to get and AudioSource From</param>
    /// <returns>An available or new AudioSource</returns>
    private static AudioSource GetNewAudioSource(AudioWrapperSource source)
    {
        AudioSource[] audioSources = source.GetComponents<AudioSource>();

        if (audioSources != null)
        {
            for (int i = 0; i < audioSources.Length; i++) //Checks for available audio sources
            {
                AudioSource thisSource = audioSources[i];

                if (!thisSource.isPlaying)//If it isn't playing
                {
                    //Debug.Log($"BALH:   {thisSource.clip.name} -> {thisSource.isPlaying}");
                    return thisSource; //Use this source
                }
            }
        }
        //Debug.Log("<<" + obj.name + ">> required an additional audio source");
        AudioSource addedSource = source.gameObject.AddComponent<AudioSource>();
        return addedSource;
    }

    /// <summary>
    /// Determins whether the audio wrapper source is active and is also not a loop.
    /// </summary>
    /// <param name="aws">the audio wrapper source to check</param>
    /// <returns>Returns whether this is true or not</returns>
    public static bool IsAudioSourceNonLoopAndActive(AudioWrapperSource aws)
    {
        if (S_AudioManager.Current.AudioSourceHasAudioQueued(aws))
        {
            return true;
        }

        foreach(AudioSource a in aws.GetComponents<AudioSource>())
        {
            //only for one time sounds that are still playing
            if (a.isPlaying && !a.loop)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Stops all audio from this audio wrapper source
    /// </summary>
    /// <param name="aws">the aduio source to all sounds from</param>
    public static void StopAllAudioSourceSounds(AudioWrapperSource aws)
    {
        foreach (AudioSource a in aws.gameObject.GetComponents<AudioSource>())
        {
            a.Stop();
        }
    }

    public static void FadeAudio(AAudioWrapperV2 aw, AudioWrapperSource aws, bool fadeInVsOut, float fadeTime)
    {
        aws.StartCoroutine(FadeAudioCR(aw, aws, fadeInVsOut, fadeTime));
    }

    private static IEnumerator FadeAudioCR(AAudioWrapperV2 aw, AudioWrapperSource aws, bool fadeInVsOut, float fadeTime)
    {
        //if (fadeInVsOut)
        //{
        //    aw.PlayAudioWrapper(aws);
        //}

        /*
        AudioSource[] sources = aws.GetComponents<AudioSource>();
        Dictionary<AudioSource, float> originalVol = new Dictionary<AudioSource, float>();
        foreach (AudioSource source in sources)
        {
            originalVol.Add(source, source.volume);

            if (fadeInVsOut)
            {
                source.volume = 0;
            }
        }*/


        float currFadeTime = 0;

        while (currFadeTime < fadeTime)
        {
            currFadeTime += Time.deltaTime;
            Debug.Log("Curr Fade Time" + currFadeTime);


            AudioSource[] sources = aws.GetCachedVolumeSources();

            foreach (AudioSource source in sources)
            {
                float ogVol = aws.GetOGVolume(source);
                float targetVol = fadeInVsOut ? ogVol : 0;
                if (currFadeTime >= fadeTime)
                {
                    source.volume = targetVol;
                    if (!fadeInVsOut)
                    {
                        source.Stop();
                    }
                    continue;
                }

                float startDB = fadeInVsOut ? 0 : ogVol;
                source.volume = Mathf.Lerp(startDB, targetVol, currFadeTime / fadeTime);
            }

            yield return null;
        }
    }
}
