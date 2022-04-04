using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using static HelperUtil;

[CreateAssetMenu(fileName = "SO_LoopAudioSettings", menuName = "ScriptableObjects/Audio/SO_LoopAudioSettings")]
public class SO_LoopAudioSettings : ScriptableObject
{
    [SerializeField]
    private List<LoopAudioWrapper> loopedWrappers = new List<LoopAudioWrapper>();

    /*
    public void SpawnAndPlayNewLoopObjectSource(GameModeEnum gameMode)
    {
        LoopAudioWrapper law = GetLAWrapper(gameMode);
        if (law == null) { return; }

        GameObject obj = new GameObject($"LoopedAudio-{gameMode}");
        AudioWrapperSource awSource = obj.AddComponent<AudioWrapperSource>();
        awSource.SetMixerGroup(law.MixerGroup);
        //Instantiate(obj);
        law.AudioWrapper.PlayAudioWrapper(awSource);
    }*/

    public LoopAudioWrapper GetAudioLoopWrapper(GameModeEnum gameMode)//, out AAudioWrapperV2 aaw, out AudioMixerGroup mix)
    {
        return GetWrapperFromFunc(
            loopedWrappers,
            law => law.GameMode,
            gameMode,
            LogEnum.NONE, null);

        //aaw = wrapper.AudioWrapper;
        //mix = wrapper.MixerGroup;
    }

    
}

[Serializable]
public class LoopAudioWrapper
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

    [SerializeField]
    private float fadeAudioInTime = 0;
    public float FadeAudioInTime => fadeAudioInTime;

    [SerializeField]
    private float fadeAudioOutTime = 0;
    public float FadeAudioOutTime => fadeAudioOutTime;
}