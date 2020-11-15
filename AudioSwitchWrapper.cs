using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "audioEventWrapper", menuName = "ScriptableObjects/AudioEventWrapper", order = 1)]
public class AudioSwitchWrapper : AAudioWrapper
{

    [Serializable]
    public class AudioWrapperSwitch
    {
        [SerializeField]
        public AAudioWrapper aAudioWrapper;
        //[SerializeField]
        //public string audioCase;

        [SerializeField]
        public List<GameInfo.INFO> gameInfo = new List<GameInfo.INFO>();
    }

    [SerializeField]
    public AAudioSwitchCase asc = null;

    [SerializeField]
    List<AudioWrapperSwitch> audioWrappersSwitches = new List<AudioWrapperSwitch>();

    override public void PlayAudioWrappers(GameObject soundObject)
    {
        string switchCase = asc.Decide();
        foreach (AudioWrapperSwitch aws in audioWrappersSwitches)
        {
            //if (aws.audioCase == switchCase)
            //{
            //    aws.aAudioWrapper.PlayAudioWrappers(soundObject);
            //}
            bool shouldPlay = true;
            foreach (GameInfo.INFO gi in aws.gameInfo)
            {
                if (!GameInfo.Current.blorp(gi))
                {
                    shouldPlay = false;
                    break;
                }
            }
            if (shouldPlay)
            {
                aws.aAudioWrapper.PlayAudioWrappers(soundObject);
            }
        }
    }
}
