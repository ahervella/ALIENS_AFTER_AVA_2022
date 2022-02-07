﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static AudioUtil;

[CreateAssetMenu(fileName = "AudioClipWrapperV2", menuName = "ScriptableObjects/Audio/AudioClipWrapperV2", order = 1)]
public class AudioClipWrapperV2 : AAudioWrapperV2
{
    [SerializeField]
    private List<AudioClip> audioClips = new List<AudioClip>();
    public List<AudioClip> AudioClips => audioClips;

    [SerializeField]
    [Range(-60f, 0f)]
    private float levelDb = -3;

    private float currLevelDb;

    [Range(-1200, 1200)]
    public int pitchCents = 0;

    [Range(0f, 6f)]
    private float volVrtnDb = 0;

    [Range(0, 1200)]
    private float pitchVrtnCents = 0;

    [SerializeField]
    private bool isRandom = true;

    [SerializeField]
    private bool loop = false;

    AudioClip cachedAudioClip = null;

    override protected void PlayAudio(GameObject soundObject, AudioMixerGroup mixerGroup)
    {
        PlayAudioClipWrapperV2(soundObject, mixerGroup);
    }

    /// <summary>
    /// Plays an AudioClip from an AudioClipWrapperV2 on the specified GameObject
    /// </summary>
    /// <param name="acw">AudioClipWrapperV2 from which a clip will be chosen and played</param>
    /// <param name="soundObject">GameObject to play from</param>
    /// <returns>Returns the AudioClip which was played</returns>
    private void PlayAudioClipWrapperV2(GameObject soundObject, AudioMixerGroup mixerGroup)
    {
        S_AudioManager.Current.PauseToggleAllAudioClipWrapperV2s -= TogglePause;
        S_AudioManager.Current.PauseToggleAllAudioClipWrapperV2s += TogglePause;

        cachedSource = GetAudioSource(soundObject);

        SourceProperties sp = soundObject.GetComponent<SourceProperties>();

        if (sp == null)
        {
            Debug.LogError("No SourceProperties Component on the gameobject: " + soundObject.name);
        }

        AudioClip clip = null;

        if (isRandom)
        {
            clip = AudioClips[Random.Range(0, AudioClips.Count - 1)];
            AudioClips.Remove(clip);
            AudioClips.Add(clip);
        }
        else
        {
            // :/
            Debug.Log(name + " IS NOT RANDOM >>:(");
        }

        float volDb = currLevelDb + Random.Range(-volVrtnDb, volVrtnDb);
        float randPitchCents = pitchCents + Random.Range(-pitchVrtnCents, pitchVrtnCents);

        AssignSourceProperties(cachedSource, mixerGroup, volDb, randPitchCents, clip);
        cachedSource.loop = loop;
        cachedSource.Play();
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

    public override void AddOffset(float offsetDb)
    {
        currLevelDb += offsetDb;
    }

    protected override void ResetLevelOffset()
    {
        currLevelDb = levelDb;
    }
}
