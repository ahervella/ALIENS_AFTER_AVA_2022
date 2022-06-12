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

    [SerializeField]
    private float bossLocalLaneChangeTime = 1f;
    public float BossLocalLaneChangeTime => bossLocalLaneChangeTime;

    [SerializeField]
    private RageValue<float> idlePhaseTime = default;

    [SerializeField]
    private RageValue<float> idlePhaseTimeOffsetRange = default;

    public float GetRandRangeIdlePhaseTime(bool rage)
    {
        float randOffset = Random.Range(
            -idlePhaseTimeOffsetRange.GetVal(rage),
            idlePhaseTimeOffsetRange.GetVal(rage));

        return randOffset + idlePhaseTime.GetVal(rage);
    }

    [SerializeField]
    private RageValue<float> shootPhaseTime = default;
    public float ShootPhaseTime(bool rage) => shootPhaseTime.GetVal(rage);

    [SerializeField]
    private RageValue<ShooterWrapper> beamShooterWrapper = null;

    public ShooterWrapper BeamShooterWrapper(bool rage)
        => beamShooterWrapper.GetVal(rage);

    [SerializeField]
    private float deathFallTime = default;
    public float DeathFallTime => deathFallTime;

    [SerializeField]
    private float droneDeathFallRandDelayRange = default;
    public float DroneDeathFallRandDelayRange => droneDeathFallRandDelayRange;
}

public enum Boss3State
{
    IDLE = 0, SHOOT = 1, DEATH = 2, START = 3
}