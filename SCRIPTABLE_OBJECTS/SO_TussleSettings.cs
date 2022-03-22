using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Video;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "SO_TussleSettings", menuName = "ScriptableObjects/StaticData/SO_TussleSettings")]
public class SO_TussleSettings : ScriptableObject
{
    [SerializeField]
    private List<TussleVideoWrapper> videoWrappers = new List<TussleVideoWrapper>();


    [SerializeField]
    private List<ButtonCharacterWrapper> buttonCharWrappers = new List<ButtonCharacterWrapper>();

    public List<InputEnum> GetListOfAvailableInputs() => buttonCharWrappers.ConvertAll(bcw => bcw.Input);

    public ButtonCharacterWrapper GetRandomCharacterWrapper()
    {
        return buttonCharWrappers[Random.Range(0, buttonCharWrappers.Count)];
    }

    [SerializeField]
    private TussleInputSequence advantageSequencePrefab = null;

    [SerializeField]
    private TussleInputSequence disadvantageSequencePrefab = null;

    public TussleInputSequence GetInputSequencePrefab(bool advantage)
    {
        return advantage ? advantageSequencePrefab : disadvantageSequencePrefab;
    }

    [SerializeField]
    private IntPropertySO currZone = null;

    [SerializeField]
    private List<TussleTimerZoneWrapper> zoneWrappers = new List<TussleTimerZoneWrapper>();

    [Serializable]
    private class TussleTimerZoneWrapper
    {
        [SerializeField]
        private int zone;
        public int Zone => zone;

        [SerializeField]
        private float timeForTussle;
        public float TimeForTussle => timeForTussle;
    }

    public TussleVideoWrapper GetTussleVideoWrapper(TussleVideoType videoType)
    {
        foreach (TussleVideoWrapper tvw in videoWrappers)
        {
            if (tvw.VideoType == videoType)
            {
                return tvw;
            }
        }
        Debug.LogError($"No video found for tussle type {videoType}");
        return null;
    }

    public float GetCurrZoneTussleTime()
    {
        foreach (TussleTimerZoneWrapper ttzw in zoneWrappers)
        {
            if (ttzw.Zone == currZone.Value)
            {
                return ttzw.TimeForTussle;
            }
        }
        Debug.LogError($"No tussle time found for zone {currZone.Value}");
        return -1;
    }
}

[Serializable]
public class TussleVideoWrapper
{
    [SerializeField]
    private TussleVideoType videoType;
    public TussleVideoType VideoType => videoType;

    [SerializeField]
    private VideoClip video = null;
    public VideoClip Video => video;

    [SerializeField]
    private bool loop;
    public bool Loop => loop;

    [SerializeField]
    private AAudioWrapperV2 audioWrapper = null;
    public AAudioWrapperV2 AudioWrapper => audioWrapper;
}


[Serializable]
public class ButtonCharacterWrapper
{
    [SerializeField]
    private InputEnum input;
    public InputEnum Input => input;

    [SerializeField]
    private string characters = string.Empty;
    public string Characters => characters;
}

public enum TussleVideoType
{
    ADV_START = 1,
    ADV_LOOP = 2,
    ADV_WIN = 3,
    ADV_LOSE = 4,
    DIS_START = 5,
    DIS_LOOP = 6,
    DIS_WIN = 7,
    DIS_LOSE = 8
}