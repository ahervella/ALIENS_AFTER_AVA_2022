using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "soundWrapper", menuName = "ScriptableObjects/SoundWrapper", order = 1)]
public class SoundWrapper : ScriptableObject
{
    public List<AudioClipWrapper> audioClipWrappers = new List<AudioClipWrapper>();
}
