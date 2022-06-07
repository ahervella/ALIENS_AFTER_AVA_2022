using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Boss3Settings", menuName = "ScriptableObjects/StaticData/SO_Boss3Settings")]
public class SO_Boss3Settings : SO_ABossSettings
{
    [SerializeField]
    private float spawnDelay = default;
    public float SpawnDelay => spawnDelay;

    [SerializeField]
    private float spawnPosTransitionTime = default;
    public float SpawnPosTransitionTime => spawnPosTransitionTime;

    [SerializeField]
    private float cannonSpawnTransitionDelay = default;
    public float CannonSpawnTransitionDelay => cannonSpawnTransitionDelay;
}

public enum Boss3State
{
    IDLE = 0
}