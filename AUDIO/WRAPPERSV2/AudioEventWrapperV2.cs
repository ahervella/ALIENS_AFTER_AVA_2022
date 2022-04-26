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
        private AAudioWrapperV2 aAudioWrapper = null;
        public AAudioWrapperV2 AAudioWrapper => aAudioWrapper;

        [SerializeField, Range(0f, 5f)]
        private float delSeconds = 0;
        public float DelSeconds => delSeconds;

        [SerializeField, Range(-60f, 20f)]
        private float secondaryOffset = 0;
        public float SecondaryOffset => secondaryOffset;
    }

    [SerializeField]
    List<SeqAudioWrapperV2> seqAudioWrappers = new List<SeqAudioWrapperV2>();

    override protected void PlayAudio(AudioWrapperSource soundObject)
    {
        foreach (SeqAudioWrapperV2 saw in seqAudioWrappers)
        {
            if (saw == null || saw.AAudioWrapper == null)
            {
                Debug.Log($"SeqAudioWrapperV2 or it's audio wrapper is null in AudioEventWrapperV2 {name}");
                continue;
            }
            saw.AAudioWrapper.AddOffset(currLevelOffsetDb);
            saw.AAudioWrapper.AddOffset(saw.SecondaryOffset);
            S_AudioManager.Current.PlayDelayed(saw.AAudioWrapper, saw.DelSeconds, soundObject, unstoppable);
        }
        ResetLevelOffset();
    }
}
