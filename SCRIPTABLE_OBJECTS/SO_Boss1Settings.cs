using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Boss1Settings", menuName = "ScriptableObjects/StaticData/SO_Boss1Settings")]
public class SO_Boss1Settings : SO_ABossSettings
{
    [SerializeField]
    private RageValue<SO_ShooterSettings> shootSettings = default;
    public SO_ShooterSettings ShootSettings(bool rage) => shootSettings.GetVal(rage);
    
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
    private RageValue<float> shootPhaseTime = default;

    [SerializeField]
    private RageValue<float> shootPhaseTimeOffsetRange = default;

    public float GetRandRangeShootPhaseTime(bool rage)
    {
        float randOffset = Random.Range(
            -shootPhaseTimeOffsetRange.GetVal(rage),
            shootPhaseTimeOffsetRange.GetVal(rage));

        return randOffset + shootPhaseTime.GetVal(rage);
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
    START = 0, SHOOT = 1, SHOOT_END = 7, IDLE = 2, LEFT = 3, RIGHT = 4, DEATH = 5, NONE = 6
}