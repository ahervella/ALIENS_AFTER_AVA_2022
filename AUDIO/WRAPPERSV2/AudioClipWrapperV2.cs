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

    [SerializeField, Range(-1200, 1200)]
    private int pitchCents = 0;

    [SerializeField, Range(0f, 6f)]
    private float volVrtnDb = 0;

    [SerializeField]
    [Range(0, 1200)]
    private float pitchVrtnCents = 0;

    [SerializeField]
    private bool loop = false;

    [NonSerialized]
    AudioClip cachedAudioClip = null;

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
        S_AudioManager.Current.PauseToggleAllAudioClipWrapperV2s -= TogglePause;
        S_AudioManager.Current.PauseToggleAllAudioClipWrapperV2s += TogglePause;

        cachedSource = GetAudioSource(soundObject);

        AudioWrapperSource awSource = soundObject.GetComponent<AudioWrapperSource>();

        if (awSource == null)
        {
            Debug.LogError("No SourceProperties Component on the gameobject: " + soundObject.name);
        }

        AudioClip clip = randomAudioClipPool[0];

        if (randomAudioClipPool.Capacity > 1)
        {
            clip = randomAudioClipPool[Random.Range(0, randomAudioClipPool.Count - 1)];
            randomAudioClipPool.Remove(clip);
            randomAudioClipPool.Add(clip);
        }

        float volDb = currLevelOffsetDb + Random.Range(-volVrtnDb, volVrtnDb);
        float randPitchCents = pitchCents + Random.Range(-pitchVrtnCents, pitchVrtnCents);

        AssignSourceProperties(cachedSource, volDb, randPitchCents, clip);
        cachedSource.loop = loop;
        cachedSource.Play();
        return cachedSource;
    }

    private void TogglePause(bool pause)
    {
        if (!Pausable())
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
        if (Pausable())
        {
            cachedSource.Pause();
        }
    }

    private bool Pausable()
    {
        return cachedSource != null && cachedSource.clip == cachedAudioClip && cachedSource.isPlaying;
    }

    private void ResumeAudio()
    {
        if (Pausable())
        {
            cachedSource.UnPause();
        }
    }
}
