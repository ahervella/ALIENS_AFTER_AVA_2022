using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "audioEventWrapper", menuName = "ScriptableObjects/AudioEventWrapper", order = 1)]
public class AudioEventWrapper : AAudioWrapper
{
    //public List<Tuple<AAudioWrapper, float>> seqAudioWrappers = new List<Tuple<AAudioWrapper, float>>();

    [Serializable]
    public class SeqAudioWrapper
    {
        [SerializeField]
        public AAudioWrapper aAudioWrapper;
        [SerializeField, Range(0f, 2f)]
        public float delSeconds = 0;
    }

    [SerializeField]
    List<SeqAudioWrapper> seqAudioWrappers = new List<SeqAudioWrapper>();

    override public void PlayAudioWrappers(GameObject soundObject)
    {
        foreach (SeqAudioWrapper saw in seqAudioWrappers)
        {
            RunnerSounds.Current.PlayDelayed(saw.aAudioWrapper, saw.delSeconds, soundObject);
        }
    }

}
