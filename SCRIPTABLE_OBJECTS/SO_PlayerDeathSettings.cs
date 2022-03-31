using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_PlayerDeathSettings", menuName = "ScriptableObjects/StaticData/SO_PlayerDeathSettings")]
public class SO_PlayerDeathSettings : ScriptableObject
{
    [SerializeField]
    private AAudioWrapperV2 deathAudio;
    public AAudioWrapperV2 DeathAudio => deathAudio;

    [SerializeField]
    private float deathCut2BlackTime = default;
    public float DeathCut2BlackTime => deathCut2BlackTime;

    [SerializeField]
    private float holdBlackTime = default;
    public float HoldBlackTime => holdBlackTime;
}
