using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "audioEventWrapperV2", menuName = "ScriptableObjects/Audio/AudioEventWrapperV2", order = 1)]
public class AudioEventWrapperV2 : AAudioWrapperV2
{

    [SerializeField]
    private bool unstoppable = false;

    // using this getter and setter, this allows the unstoppable value given above to be overwritten by a parent wrapper
    public bool Unstoppable
    {
        get
        {
            return unstoppable;
        }

        private set
        {
            unstoppable = value;
        }
    }

    [Serializable]
    public class SeqAudioWrapperV2
    {
        [SerializeField]
        public AAudioWrapperV2 aAudioWrapper;

        [SerializeField, Range(0f, 2f)]
        public float delSeconds = 0;

        [SerializeField, Range(-60f, 0f)]
        public float secondaryOffset = 0;
    }

    [SerializeField]
    List<SeqAudioWrapperV2> seqAudioWrappers = new List<SeqAudioWrapperV2>();

    override protected void PlayAudio(GameObject soundObject, AudioMixerGroup mixerGroup)
    {
        foreach (SeqAudioWrapperV2 saw in seqAudioWrappers)
        {
            saw.aAudioWrapper.AddOffset(currLevelOffsetDb);
            saw.aAudioWrapper.AddOffset(saw.secondaryOffset);
            S_AudioManager.Current.PlayDelayed(saw.aAudioWrapper, saw.delSeconds, soundObject, mixerGroup, Unstoppable);
        }
        ResetLevelOffset();
    }
}
