using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioSwitchCaseWrapper", menuName = "ScriptableObjects/Audio/AudioSwitchCaseWrapper", order = 1)]
public class AudioSwitchCaseWrapper : AAudioContainer
{
    [SerializeField, GetSet("InfoType")]
    private GameInfo.INFO infoType = GameInfo.INFO.NONE;
    public GameInfo.INFO InfoType
    {
        get
        {
            return infoType;
        }
        set
        {
            ParseGameInfoDetails(value);
            infoType = value;
        }
    }

    private void ParseGameInfoDetails(GameInfo.INFO infoType)
    {
        switchCases.Clear();
        GameInfo.GameInfoTypeDetails details = GameInfo.GetGameInfoTypeDetails(infoType);
        if (details == null)
        {
            return;
        }

        switchCaseFunction = details.GetState;

        foreach(int id in details.StateNameDict.Keys)
        {
            string name;
            details.StateNameDict.TryGetValue(id, out name);
            switchCases.Add(new SwitchCaseDisplayWrapper(name, id));
        }
    }

    [SerializeField]
    private List<SwitchCaseDisplayWrapper> switchCases;// = new AudioSwitchCaseWrapper();

    private Func<int> switchCaseFunction;


    [Serializable]
    private class SwitchCaseDisplayWrapper
    {
        public SwitchCaseDisplayWrapper(string switchCaseName, int scenarioId)
        {
            switchCase = switchCaseName;
            this.scenarioId = scenarioId;
        }

        public int scenarioId { get; private set; }

        [SerializeField, ReadOnly]
        private string switchCase;
        public string SwitchCaseName => switchCase;

        [SerializeField]
        private List<AudioWrapperEntry> audioWrapperEntries;
        public List<AudioWrapperEntry> AudioWrapperEntries => audioWrapperEntries;


    }

    [Serializable]
    public class AudioWrapperEntry
    {
        [SerializeField]
        public AAudioWrapper aAudioWrapper;

        [SerializeField, Range(-60f, 0f)]
        public float secondaryOffset = 0;

    }

    private void OnEnable()
    {
        base.OnEnable();
        switchCaseFunction = GameInfo.GetGetterMethod(infoType);
    }

    public override void PlayAudioWrappers(GameObject soundObject)
    {
        int currCase = switchCaseFunction();

        foreach (SwitchCaseDisplayWrapper switchCase in switchCases)
        {
            if (switchCase.scenarioId == currCase)
            {
                foreach (AudioWrapperEntry awe in switchCase.AudioWrapperEntries)
                {
                    awe.aAudioWrapper.AddOffset(LevelOffsetDb);
                    awe.aAudioWrapper.AddOffset(awe.secondaryOffset);
                    awe.aAudioWrapper.PlayAudioWrappers(soundObject);
                }
                ResetLevelOffset();
                return;
            }
        }
    }

    public override void AddOffset(float offsetDb)
    {
        LevelOffsetDb += offsetDb;
    }
}
