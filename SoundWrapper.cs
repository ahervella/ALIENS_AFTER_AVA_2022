using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SoundWrapper", order = 1)]
public class SoundWrapper : ScriptableObject
{
    public AudioClip audioClip;
    public float vol = 1;
    public float pitch = 1;
    public float volVariation = 0;
    public float pitchVariation = 0;
}
