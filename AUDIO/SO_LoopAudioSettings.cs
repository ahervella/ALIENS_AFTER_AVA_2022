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
    private List<LoopAudioWrapper> audioLoopWrappers = new List<LoopAudioWrapper>();


    public LoopAudioWrapper GetAudioLoopWrapper(GameModeEnum gameMode, ZonePhaseEnum zonePhase, int zone)
    {
        List<LoopAudioWrapper> matchGameMode = audioLoopWrappers.FindAll(law => law.GameMode == gameMode);
        if (matchGameMode.Count == 0) { return null; }

        LoopAudioWrapper soleZonePhaseWrapper = matchGameMode.Find(law => law.ZonePhase == ZonePhaseEnum.NONE);
        if (soleZonePhaseWrapper != null)
        {
            return soleZonePhaseWrapper;
        }

        List<LoopAudioWrapper> zonePhaseWrappers = matchGameMode.FindAll(law => law.ZonePhase == zonePhase);

        LoopAudioWrapper soleZoneWrapper = zonePhaseWrappers.Find(law => !law.ZoneDependant);
        if (soleZoneWrapper != null)
        {
            return soleZoneWrapper;
        }

        return zonePhaseWrappers.Find(law => law.Zone == zone);
    }
}

[Serializable]
public class LoopAudioWrapper
{
    [SerializeField]
    private GameModeEnum gameMode = GameModeEnum.PLAY;
    public GameModeEnum GameMode => gameMode;

    [SerializeField]
    private ZonePhaseEnum zonePhase = default;
    public ZonePhaseEnum ZonePhase => zonePhase;

    [SerializeField]
    private bool zoneDependant = false;
    public bool ZoneDependant => zoneDependant;

    [SerializeField]
    private int zone = default;
    public int Zone => zoneDependant ? zone : -1;

    [SerializeField]
    private bool silenceForThisConfig = false;
    public bool SilenceForThisConfig => silenceForThisConfig;

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
