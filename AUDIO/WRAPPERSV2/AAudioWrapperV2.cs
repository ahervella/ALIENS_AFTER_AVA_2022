using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public abstract class AAudioWrapperV2 : ScriptableObject
{
    [SerializeField]
    [Range(-60f, 20f)]
    protected float levelOffsetDb = 0;

    [NonSerialized]
    protected float currLevelOffsetDb = 0;

    [NonSerialized]
    protected AudioSource cachedSource = null;

    /// <summary>
    /// Plays an AudioWrapper on the specified GameObject
    /// </summary>
    /// <param name="aw">AudioWrapper to be played</param>
    /// <param name="soundObject">GameObject to be played from</param>
    public void PlayAudioWrapper(AudioWrapperSource soundObject, bool stopObjectSounds = false)
    {
        if (soundObject == null)
        {
            Debug.LogError("Target sound Object does not exist");
            return;
        }

        if (stopObjectSounds)
        {
            S_AudioManager.Current.StopAllDelayedSounds(soundObject);
        }

        currLevelOffsetDb += levelOffsetDb;

        PlayAudio(soundObject);

        ResetLevelOffset();
        //Debug.Log("AudioWrapper << " + aw + " >> was played");
    }


    protected abstract void PlayAudio(AudioWrapperSource soundObject);

    // Adds an offset to the wrapper's volume level
    public void AddOffset(float offsetDb)
    {
        currLevelOffsetDb += offsetDb;
    }

    // Resets the wrapper's volume level (to be used after playing all included wrappers or clips)
    //This prevents offsets from compounding
    public void ResetLevelOffset()
    {
        currLevelOffsetDb = 0;// levelOffsetDb;
    }
}