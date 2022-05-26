using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Boss2Settings", menuName = "ScriptableObjects/StaticData/SO_Boss2Settings")]
public class SO_Boss2Settings : SO_ABossSettings
{
    [SerializeField]
    private float flyOverTime = 1f;
    public float FlyOverTime => flyOverTime;

    [SerializeField]
    private float spawnFlyOverDipFloorHeight = 1f;
    public float SpawnFlyOverDipFloorHeight => spawnFlyOverDipFloorHeight;

    [SerializeField]
    private float spawnDelay = 2f;
    public float SpawnDelay => spawnDelay;

    [SerializeField]
    private RageValue<int> minIdleFlybys = null;

    [SerializeField]
    private RageValue<int> maxIdleFlybys = null;

    public int GetRandFlyByCount(bool rage)
    {
        return Random.Range(minIdleFlybys.GetVal(rage), maxIdleFlybys.GetVal(rage) + 1);
    }

    [SerializeField]
    private RageValue<float> fullFlybyTime = null;
    public RageValue<float> FullFlybyTime => fullFlybyTime;

    [SerializeField]
    private int flybyStartTileDelta = 4;
    public int FlybyStartTileDelta => flybyStartTileDelta;

    [SerializeField]
    private int flybyDipFloorHeight = 1;
    public int FlybyDipFloorHeight => flybyDipFloorHeight;

    [SerializeField]
    private float attackStartTileDist = 1f;
    public float AttackStartTileDist => attackStartTileDist;

    [SerializeField]
    private RageValue<float> attackZTileSpeed = null;
    public RageValue<float> AttackZTileSpeed => attackZTileSpeed;

    [SerializeField]
    private float postAttack2IdleDelay = 1f;
    public float PosAttack2IdleDelay => postAttack2IdleDelay;

    [SerializeField]
    private float deathRiseTime = 1f;
    public float DeathRiseTime => deathRiseTime;
}

public enum Boss2State
{
    FLYOVER_STRAIGHT = 0,
    FLYOVER_RISE = 1,
    ATTACK_START_LEFT = 2,
    ATTACK_START_RIGHT = 3,
    IDLE_FLY_RIGHT = 4,
    IDLE_FLY_LEFT = 5,
    SPREAD_WINGS = 11,
    SPREAD_WINGS_MIDDLE_LOW = 6,
    SPREAD_WINGS_MIDDLE_HIGH = 10,
    SPREAD_WINGS_LEFT = 7,
    SPREAD_WINGS_RIGHT = 8,
    NONE = 9
}
