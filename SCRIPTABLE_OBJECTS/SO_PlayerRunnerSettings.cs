using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_PlayerRunnerSettings", menuName = "ScriptableObjects/StaticData/SO_PlayerRunnerSettings")]
public class SO_PlayerRunnerSettings : ScriptableObject
{
    [SerializeField]
    private float lifeRecoveryTime;
    public float LifeRecoveryTime => lifeRecoveryTime;

    [SerializeField]
    private float startingEnergyBlocks;
    public float StartingEnergyBlocks => startingEnergyBlocks;
}
