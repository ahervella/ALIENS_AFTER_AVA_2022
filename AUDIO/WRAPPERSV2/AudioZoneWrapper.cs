using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "azw_", menuName = "ScriptableObjects/Audio/AudioZoneWrapper")]
public class AudioZoneWrapper : AAudioWrapperV2
{
    [SerializeField]
    private List<ZoneWrapper> wrappers = new List<ZoneWrapper>();

    [Serializable]
    private class ZoneWrapper
    {
        [SerializeField]
        private int zone = 0;
        public int Zone => zone;

        [SerializeField]
        private AAudioWrapperV2 audioWrapper = null;
        public AAudioWrapperV2 AudioWrapper => audioWrapper;
    }

    [NonSerialized]
    private int cachedCurrZone = -1;

    [NonSerialized]
    private AAudioWrapperV2 cachedCurrAudioWrapper = null;

    protected override void PlayAudio(AudioWrapperSource soundObject)
    {
        GetAudioWrapper().AddOffset(currLevelOffsetDb);
        GetAudioWrapper().PlayAudioWrapper(soundObject);
    }

    private AAudioWrapperV2 GetAudioWrapper()
    {
        int zone = S_AudioManager.Current.CurrZone.Value;
        if (cachedCurrZone == zone && cachedCurrAudioWrapper != null)
        {
            return cachedCurrAudioWrapper;
        }

        foreach(ZoneWrapper zw in wrappers)
        {
            if (zw.Zone == zone)
            {
                cachedCurrZone = zw.Zone;
                cachedCurrAudioWrapper = zw.AudioWrapper;
                return cachedCurrAudioWrapper;
            }
        }

        Debug.LogError($"No AudioWrapper found for zone {zone} for ZoneWrapper {name}");
        return null;
    }
}


