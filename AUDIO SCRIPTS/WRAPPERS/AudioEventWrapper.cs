using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "audioEventWrapper", menuName = "ScriptableObjects/AudioEventWrapper", order = 1)]
public class AudioEventWrapper : AAudioContainer
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
    public class SeqAudioWrapper
    {
        [SerializeField]
        public AAudioWrapper aAudioWrapper;

        [SerializeField, Range(0f, 2f)]
        public float delSeconds = 0;

        [SerializeField, Range(-60f, 0f)]
        public float secondaryOffset = 0;
    }

    [SerializeField]
    List<SeqAudioWrapper> seqAudioWrappers = new List<SeqAudioWrapper>();

    override public void PlayAudioWrappers(GameObject soundObject)
    {
        foreach (SeqAudioWrapper saw in seqAudioWrappers)
        {
            saw.aAudioWrapper.AddOffset(LevelOffsetDb);
            saw.aAudioWrapper.AddOffset(saw.secondaryOffset);
            RunnerSounds.Current.PlayDelayed(saw.aAudioWrapper, saw.delSeconds, soundObject, Unstoppable);
        }
        ResetLevelOffset();
    }

    public override void AddOffset(float offsetDb)
    {
        LevelOffsetDb += offsetDb;
    }
}
