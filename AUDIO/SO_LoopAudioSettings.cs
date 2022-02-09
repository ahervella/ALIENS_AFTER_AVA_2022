using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

[CreateAssetMenu(fileName = "SO_LoopAudioSettings", menuName = "ScriptableObjects/Audio/SO_LoopAudioSettings")]
public class SO_LoopAudioSettings : ScriptableObject
{
    [SerializeField]
    private List<LoopAudioWrapper> loopedWrappers = new List<LoopAudioWrapper>();


    public void SpawnAndPlayNewLoopObjectSource(GameModeEnum gameMode)
    {
        LoopAudioWrapper law = GetLAWrapper(gameMode);
        if (law == null) { return; }

        GameObject obj = new GameObject($"LoopedAudio-{gameMode}");
        AudioWrapperSource awSource = obj.AddComponent<AudioWrapperSource>();
        awSource.SetMixerGroup(law.MixerGroup);
        //Instantiate(obj);
        law.AudioWrapper.PlayAudioWrapper(awSource);
    }

    private LoopAudioWrapper GetLAWrapper(GameModeEnum gameMode)
    {
        foreach(LoopAudioWrapper law in loopedWrappers)
        {
            if (law.GameMode == gameMode)
            {
                return law;
            }
        }
        return null;
    }

    [Serializable]
    private class LoopAudioWrapper
    {
        [SerializeField]
        private GameModeEnum gameMode = GameModeEnum.PLAY;
        public GameModeEnum GameMode => gameMode;

        [SerializeField]
        private AAudioWrapperV2 audioWrapper = null;
        public AAudioWrapperV2 AudioWrapper => audioWrapper;

        [SerializeField]
        private AudioMixerGroup mixerGroup;
        public AudioMixerGroup MixerGroup => mixerGroup;
    }
}
