using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static AudioUtil;
using System;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "acw_", menuName = "ScriptableObjects/Audio/AudioClipWrapperV2")]
public class AudioClipWrapperV2 : AAudioWrapperV2
{
    [SerializeField]
    private List<AudioClip> randomAudioClipPool = new List<AudioClip>();

    [NonSerialized]
    private List<AudioClip> randAudioClipPoolCopy;

    [SerializeField, Range(-1200, 1200)]
    private int pitchCents = 0;

    [SerializeField, Range(0f, 6f)]
    private float volVrtnDb = 0;

    [SerializeField]
    [Range(0, 1200)]
    private float pitchVrtnCents = 0;

    [SerializeField]
    private bool loop = false;

    [SerializeField]
    private bool pauseAudioOnGamePause = true;

    [SerializeField]
    private bool randAvoidLastTwoPlayed = true;

    [NonSerialized]
    private AudioClip lastClipPlayed = null;

    [NonSerialized]
    private AudioClip secondLastClipPlayed = null;

    public void SetToLoop()
    {
        loop = true;
    }

    override protected void PlayAudio(AudioWrapperSource soundObject)
    {
        PlayAudioClipWrapperV2(soundObject);
    }

    /// <summary>
    /// Plays an AudioClip from an AudioClipWrapperV2 on the specified GameObject
    /// </summary>
    /// <param name="acw">AudioClipWrapperV2 from which a clip will be chosen and played</param>
    /// <param name="soundObject">GameObject to play from</param>
    /// <returns>Returns the AudioClip which was played</returns>
    private AudioSource PlayAudioClipWrapperV2(AudioWrapperSource soundObject)
    {
        if (pauseAudioOnGamePause)
        {
            S_AudioManager.Current.PauseToggleAllAudioClipWrapperV2s -= TogglePause;
            S_AudioManager.Current.PauseToggleAllAudioClipWrapperV2s += TogglePause;
        }

        AudioWrapperSource awSource = soundObject.GetComponent<AudioWrapperSource>();

        if (awSource == null)
        {
            Debug.LogError("No SourceProperties Component on the gameobject: " + soundObject.name);
        }

        if (randomAudioClipPool.Count > 3 && lastClipPlayed != null)
        {
            randAudioClipPoolCopy = new List<AudioClip>(randomAudioClipPool);
            randAudioClipPoolCopy.Remove(lastClipPlayed);
            if (randAvoidLastTwoPlayed && randomAudioClipPool.Count > 4)
            {
                randAudioClipPoolCopy.Remove(secondLastClipPlayed);
            }
            secondLastClipPlayed = lastClipPlayed;
            lastClipPlayed = randAudioClipPoolCopy[Random.Range(0, randAudioClipPoolCopy.Count)];
        }
        else
        {
            lastClipPlayed = randomAudioClipPool[Random.Range(0, randomAudioClipPool.Count)];
        }

        float volDb = currLevelOffsetDb + Random.Range(-volVrtnDb, volVrtnDb);
        float randPitchCents = pitchCents + Random.Range(-pitchVrtnCents, pitchVrtnCents);

        cachedSource = AssignWrapperSourceProperties(soundObject, volDb, randPitchCents, lastClipPlayed);
        cachedSource.loop = loop;
        cachedSource.Play();
        return cachedSource;
    }

    private void TogglePause(bool pause)
    {
        if (!IsCorrectAudio())
        {
            S_AudioManager.Current.PauseToggleAllAudioClipWrapperV2s -= TogglePause;
            return;
        }

        if (pause)
        {
            PauseAudio();
        }
        else
        {
            ResumeAudio();
        }
    }

    private void PauseAudio()
    {
        if (IsCorrectAudio() && cachedSource.isPlaying)
        {
            cachedSource.Pause();
        }
    }

    private bool IsCorrectAudio()
    {
        return cachedSource != null && cachedSource.clip == lastClipPlayed;
    }

    private void ResumeAudio()
    {
        if (IsCorrectAudio() && !cachedSource.isPlaying)
        {
            cachedSource.UnPause();
        }
    }
}
