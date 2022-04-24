using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_RunTransitionSettings", menuName = "ScriptableObjects/StaticData/SO_RunTransitionSettings")]
public class SO_RunTransitionSettings : ScriptableObject
{
    [SerializeField]
    private float blackHoldOnLoad = 1f;
    public float BlackHoldOnLoad => blackHoldOnLoad;

    [SerializeField]
    private float fadeInTime = 1f;
    public float FadeInTime => fadeInTime;
}
