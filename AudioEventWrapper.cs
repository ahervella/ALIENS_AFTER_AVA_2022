﻿using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "audioEventWrapper", menuName = "ScriptableObjects/AudioEventWrapper", order = 1)]
public class AudioEventWrapper : AAudioContainer
{
    //public List<Tuple<AAudioWrapper, float>> seqAudioWrappers = new List<Tuple<AAudioWrapper, float>>();

    [SerializeField]
    private bool unstoppable = false;

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

    public void Awake()
    {
        ResetLevelOffset();
    }

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
