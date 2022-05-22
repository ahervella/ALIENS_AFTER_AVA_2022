using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_OrbOscillationSettings", menuName = "ScriptableObjects/StaticData/SO_OrbOscillationSettings")]
public class SO_OrbOscillationSettings : ScriptableObject
{
    [SerializeField]
    private float oscillationDisplacement = 2f;
    public float OscillationDisplacement => oscillationDisplacement;

    [SerializeField]
    private float oscillationTime = 1f;
    public float OscillationTime => oscillationTime;

    [SerializeField]
    private float upDownMoveTime = 1f;
    public float UpDownMoveTime => upDownMoveTime;
}
