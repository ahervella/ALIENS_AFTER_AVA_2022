using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "SO_Boss1Settings", menuName = "ScriptableObjects/StaticData/SO_Boss1Settings")]
public class SO_Boss1Settings : SO_ABossSettings
{
    [SerializeField]
    private SO_DeveloperToolsSettings devTools = null;

    [SerializeField]
    private RageValue<ShooterWrapper> shootWrapper = null;
    [SerializeField]
    private RageFloatPSO dev_shootDelayDelta = null;

    [NonSerialized]
    private RageValue<ShooterWrapper> dev_cachedShooterWrapper = null;

    public ShooterWrapper ShootWrapper(bool rage)
    {
        if (dev_shootDelayDelta == null)
        {
            return shootWrapper.GetVal(rage);
        }

        if (dev_cachedShooterWrapper == null)
        {
            ShooterWrapper prerageWrapper = new ShooterWrapper(
                   shootWrapper.GetVal(false).WeaponFirePrefab,
                   shootWrapper.GetVal(false).DelayTime + dev_shootDelayDelta.Value.GetVal(false));

            ShooterWrapper rageWrapper = new ShooterWrapper(
                shootWrapper.GetVal(true).WeaponFirePrefab,
                shootWrapper.GetVal(true).DelayTime + dev_shootDelayDelta.Value.GetVal(true));


            dev_cachedShooterWrapper = new RageValue<ShooterWrapper>(
                prerageWrapper, rageWrapper);
        }

        return dev_cachedShooterWrapper.GetVal(rage);
    }

    
    [SerializeField]
    private RageValue<float> randDelayRangeBetweenFires = default;
    [SerializeField]
    private RageFloatPSO dev_fireShotRandDelayRange = null;

    public float GetRandDelayRangeBetweenFires(bool rage) =>
        randDelayRangeBetweenFires.GetVal(rage) + dev_fireShotRandDelayRange?.Value.GetVal(rage)
        ?? randDelayRangeBetweenFires.GetVal(rage);


    [SerializeField]
    private RageValue<int> shootRounds = default;
    public int ShootRounds(bool rage) => shootRounds.GetVal(rage);

    [SerializeField]
    private float spawnYPos = default;
    public float SpawnYPos => spawnYPos;

    [SerializeField]
    private float spawnTime = 3f;
    public float SpawnTime => spawnTime;

    [SerializeField]
    private float spawnDelay = 2f;
    public float SpawnDelay => spawnDelay;

    [SerializeField]
    private RageValue<float> idlePhaseTime = default;

    [SerializeField]
    private RageValue<float> idlePhaseTimeOffsetRange = default;

    [SerializeField]
    private RageFloatPSO dev_firePhaseDelayDelta = null;

    public float GetRandRangeIdlePhaseTime(bool rage)
    {
        float randOffset = Random.Range(
            -idlePhaseTimeOffsetRange.GetVal(rage),
            idlePhaseTimeOffsetRange.GetVal(rage));

        float time = randOffset + idlePhaseTime.GetVal(rage);

        return time + dev_firePhaseDelayDelta?.Value.GetVal(rage) ?? time;
    }

    [SerializeField]
    private RageValue<int> bulletsPerShot = default;
    public int BulletsPerShot(bool rage) => bulletsPerShot.GetVal(rage);

    [SerializeField]
    private float heightOfBullet = 5f;
    public float HeightOfBullet => heightOfBullet;
}

public enum Boss1State
{
    START = 0,
    SHOOT = 1,
    SHOOT_END = 7,
    IDLE = 2,
    LEFT = 3,
    RIGHT = 4,
    DEATH = 5,
    RAGE = 8,
    NONE = 6
}