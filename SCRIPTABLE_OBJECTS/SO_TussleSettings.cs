using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Video;
using Random = UnityEngine.Random;
using static HelperUtil;

[CreateAssetMenu(fileName = "SO_TussleSettings", menuName = "ScriptableObjects/StaticData/SO_TussleSettings")]
public class SO_TussleSettings : ScriptableObject
{
    [SerializeField]
    private float showSequenceDelay = 1f;
    public float ShowSequenceDelay => showSequenceDelay;

    [SerializeField]
    private SO_LayerSettings layerSettings;
    public SO_LayerSettings LayerSettings => layerSettings;

    [SerializeField]
    private float tussleHazardCleanUpTileDist = 1f;
    public float TussleHazardCleanUpTileDist => tussleHazardCleanUpTileDist;

    [SerializeField]
    private BoolDelegateSO tussleHazardCleanUpDelegate = null;
    public BoolDelegateSO TussleHazardCleanUpDelegate => tussleHazardCleanUpDelegate;

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
        return GetWrapperFromFunc(videoWrappers, tvw => tvw.VideoType, videoType, LogEnum.ERROR, null);
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

    //TODO: make this and the alien damage one an enum
    [SerializeField]
    private bool takeDamageAfterDelay = false;
    public bool TakeDamageAfterDelay => takeDamageAfterDelay;

    [SerializeField]
    private bool alienDamageAfterDelay = false;
    public bool AlienDamageAfterDelay => alienDamageAfterDelay;

    [SerializeField]
    private float damageDelay = 0f;
    public float DamageDelay => damageDelay;
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