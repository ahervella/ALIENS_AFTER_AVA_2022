using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "SO_MainMenuSettings", menuName = "ScriptableObjects/StaticData/SO_MainMenuSettings")]
public class SO_MainMenuSettings : ScriptableObject
{
    [SerializeField]
    private AAudioWrapperV2 mainMenuAudio = null;
    public AAudioWrapperV2 MainMenuAudio => mainMenuAudio;

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    [SerializeField]
    private MainMenuTimingWrapper onLoadFromBoot = null;

    [SerializeField]
    private MainMenuTimingWrapper onLoadFromRun = null;

    public MainMenuTimingWrapper GetTimingWrapper()
    {
        if (currGameMode.PrevValue == GameModeEnum.BOOT)
        {
            return onLoadFromBoot;
        }
        return onLoadFromRun;
    }

    [SerializeField]
    private AAudioWrapperV2 mainMenuVox = null;
    public AAudioWrapperV2 MainMenuVox => mainMenuVox;

    [SerializeField]
    private float minVoxDelay = default;

    [SerializeField]
    private float maxVoxDelay = default;

    public float GetRandVoxDelay()
    {
        return Random.Range(minVoxDelay, maxVoxDelay);
    }
}

[Serializable]
public class MainMenuTimingWrapper
{
    [SerializeField]
    private float videoDelayFromBlack = default;
    public float VideoDelayFromBlack => videoDelayFromBlack;

    [SerializeField]
    private float videoFadeInTime = default;
    public float VideoFadeInTime => videoFadeInTime;

    [SerializeField]
    private float titleDelayFromVideoEnd = default;
    public float TitleTotalDelay => videoDelayFromBlack + videoFadeInTime + titleDelayFromVideoEnd;

    [SerializeField]
    private float titleFadeInTime = default;
    public float TitleFadeInTime => titleFadeInTime;

    [SerializeField]
    private float buttonPromptDelayFromTitle = default;
    public float ButtonPromptTotalDelay => TitleTotalDelay + titleFadeInTime + buttonPromptDelayFromTitle;

    [SerializeField]
    private float buttonPromptFadeInTime = default;
    public float ButtonPromptFadeInTime => buttonPromptFadeInTime;
}
