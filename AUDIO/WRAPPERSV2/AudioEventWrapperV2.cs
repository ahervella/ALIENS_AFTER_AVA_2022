using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "aew_", menuName = "ScriptableObjects/Audio/AudioEventWrapperV2")]
public class AudioEventWrapperV2 : AAudioWrapperV2
{

    [SerializeField]
    private bool unstoppable = false;

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

    override protected void PlayAudio(AudioWrapperSource soundObject)
    {
        foreach (SeqAudioWrapperV2 saw in seqAudioWrappers)
        {
            if (saw == null)
            {
                Debug.Log($"SeqAudioWrapperV2 is null in AudioEventWrapperV2 {name}");
                continue;
            }
            saw.aAudioWrapper.AddOffset(currLevelOffsetDb);
            saw.aAudioWrapper.AddOffset(saw.secondaryOffset);
            S_AudioManager.Current.PlayDelayed(saw.aAudioWrapper, saw.delSeconds, soundObject, unstoppable);
        }
        ResetLevelOffset();
    }
}
