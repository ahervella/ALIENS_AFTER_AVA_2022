using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SO_MixerEffectSettings", menuName = "ScriptableObjects/Audio/SO_MixerEffectSettings")]
public class SO_MixerEffectSettings : ScriptableObject
{
    [SerializeField]
    List<MixerEffectWrapper> mixerWrappers = new List<MixerEffectWrapper>();

    /// <summary>
    /// Gets the MixerEffectsSettings that matches the mixer type
    /// </summary>
    /// <param name="mixerType">mixer type to match</param>
    /// <returns></returns>
    public void SetMixerEffectSnapshot(MixerEffectEnum mixerType)
    {
        foreach (var mix in mixerWrappers)
        {
            if (mix.MixerType == mixerType)
            {
                mix.SetAsCurrentSnapshot();
                return;
            }
        }

        Debug.LogError("You suck there's no mixer for this mixer scenario type: " + mixerType);
    }


    [Serializable]
    private class MixerEffectWrapper
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
}


