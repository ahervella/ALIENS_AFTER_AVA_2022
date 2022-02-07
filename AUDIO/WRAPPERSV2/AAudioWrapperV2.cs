using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public abstract class AAudioWrapperV2 : ScriptableObject
{
    protected AudioSource cachedSource = null;

    /// <summary>
    /// Plays an AudioWrapper on the specified GameObject
    /// </summary>
    /// <param name="aw">AudioWrapper to be played</param>
    /// <param name="soundObject">GameObject to be played from</param>
    public void PlayAudioWrapper(GameObject soundObject, AudioMixerGroup mixerGroup = null)
    {
        if (soundObject == null)
        {
            Debug.LogError("Target sound Object does not exist");
            return;
        }
        S_AudioManager.Current.StopAllDelayedSounds(soundObject);
        PlayAudio(soundObject, mixerGroup);

        ResetLevelOffset();
        //Debug.Log("AudioWrapper << " + aw + " >> was played");
    }


    abstract protected void PlayAudio(GameObject soundObject, AudioMixerGroup mixerGroup);

    // Adds an offset to the wrapper's volume level
    abstract public void AddOffset(float offsetDb);

    // Resets the wrapper's volume level (to be used after playing all included wrappers or clips)
    //This prevents offsets from compounding
    protected abstract void ResetLevelOffset();

    protected void Initialize()
    {
        ResetLevelOffset();
    }
}