using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AAudioSwitchCaseWrapper", menuName = "ScriptableObjects/AAudioSwitchCaseWrapper", order = 1)]
public class AAudioSwitchCaseWrapper : AAudioWrapper
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
            parseGameInforDetails(value);
            infoType = value;
        }
    }

    private void parseGameInforDetails(GameInfo.INFO infoType)
    {
        switchCases.Clear();
        GameInfo.GameInfoTypeDetails details = GameInfo.GetGameInfoTypeDetails(infoType);
        if (details == null)
        {
            return;
        }

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
        private List<AAudioWrapper> aaudioWrappers;
        public List<AAudioWrapper> AAWrapers => aaudioWrappers;

    }




    public override void PlayAudioWrappers(GameObject soundObject)
    {
        int currCase = switchCaseFunction();

        foreach (SwitchCaseDisplayWrapper switchCase in switchCases)
        {
            if (switchCase.scenarioId == currCase)
            {
                foreach (AAudioWrapper aaw in switchCase.AAWrapers)
                {
                    aaw.PlayAudioWrappers(soundObject);
                }
                return;
            }
        }
    }

}
