using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

[Serializable]
public class SO_MixerEffectWrapper : ScriptableObject
{
    [SerializeField]
    private MixerEffectEnum mixerType = MixerEffectEnum.NONE;
    public MixerEffectEnum MixerType => mixerType;

    [SerializeField]
    private AudioMixerSnapshot mixerSnapshot;
    public AudioMixerSnapshot MixerSnapshot => mixerSnapshot;

    [SerializeField]
    [Range(0f, 10f)]
    private float transitionTime = 1;

    public void SetAsCurrentSnapshot()
    {
        if (mixerSnapshot == null)
        {
            Debug.LogError("This MixerEffectsSetting doesn't have a snapshot for: " + mixerType);
            return;
        }

        mixerSnapshot.TransitionTo(transitionTime);
    }
}
