using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "audioClipWrapper", menuName = "ScriptableObjects/AudioClipWrapper", order = 1)]
public class AudioClipWrapper : AAudioWrapper
{
    public List<AudioClip> audioClips = new List<AudioClip>();

    [Range(-60f, 0f)]
    public float levelDb = -3;
    [Range(-1200, 1200)]
    public int pitchCents = 0;

    [Range(0f, 6f)]
    public float volVrtnDb = 0;
    [Range(0, 1200)]
    public float pitchVrtnCents = 0;

    public bool isRandom = true;

    override public void PlayAudioWrappers(GameObject soundObject)
    {
        RunnerSounds.Current.PlayAudioClip(this, soundObject);
    }
}
