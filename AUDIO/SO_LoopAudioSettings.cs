using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "SO_LoopAudioSettings", menuName = "ScriptableObjects/Audio/SO_LoopAudioSettings")]
public class SO_LoopAudioSettings : ScriptableObject
{
    [SerializeField]
    private List<AudioClipWrapperV2> acws = new List<AudioClipWrapperV2>();
    public List<AudioClipWrapperV2> ACWs => acws;

    [SerializeField]
    private AudioMixerGroup mixerGroup;
    public AudioMixerGroup MixerGroup => mixerGroup;

    public void PlayAllACWs(GameObject soundObject)
    {
        foreach (AudioClipWrapperV2 acw in acws)
        {
            acw.PlayAudioWrapper(soundObject, mixerGroup);
        }
    }
}
