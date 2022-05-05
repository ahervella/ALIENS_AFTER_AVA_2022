using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static HelperUtil;

public abstract class SO_ABossSettings : ScriptableObject
{
    [SerializeField]
    private int startingHealth;
    [SerializeField]
    private IntPropertySO dev_startingHealthDelta = null;

    public int StartingHealth =>
        startingHealth + dev_startingHealthDelta?.Value ?? startingHealth;

    [SerializeField]
    private int rageHealthThreshold = default;
    [SerializeField]
    private IntPropertySO dev_rageHealthThresholdDelta = null;
    public virtual float RageHealthThreshold =>
        rageHealthThreshold + dev_rageHealthThresholdDelta?.Value ?? rageHealthThreshold;

    [SerializeField]
    private int hitBoxTileWidth = 1;
    public int HitBoxTileWidth => hitBoxTileWidth;

    [SerializeField]
    private int spawnTileRowsAway = 5;
    public int SpawnTileRowsAway => spawnTileRowsAway;

    [SerializeField]
    private BossSpawnEnum spawnType = BossSpawnEnum.INDEPENDENT;
    public BossSpawnEnum SpawnType => spawnType;

    [SerializeField]
    private AFillBarManagerBase healthBarPrefab = null;
    public AFillBarManagerBase HealthBarPrefab => healthBarPrefab;

    [SerializeField]
    private float healthBarSpawnDelay = 2f;
    public float HealthBarSpawnDelay => healthBarSpawnDelay;

    [SerializeField]
    private float tearDownDelayPostDeath = 1f;
    public float TearDownDelayPostDeath => tearDownDelayPostDeath;

    [SerializeField]
    private AAudioWrapperV2 spawnAudioWrapper = null;
    public AAudioWrapperV2 SpawnAudioWrapper => spawnAudioWrapper;

    [SerializeField]
    private float spawnAudioDelay = 1f;
    public float SpawnAudioDelay => spawnAudioDelay;

    [SerializeField]
    private AAudioWrapperV2 hurtAudioWrapper = null;
    public AAudioWrapperV2 HurtAudioWrapper => hurtAudioWrapper;
}

//TODO: Change to a general EnumValueWrapper to be able to have multiple boss
//phase with sub phases
[Serializable]
public class RageValue<T>
{
    [SerializeField]
    private T prerageVal = default;

    [SerializeField]
    private T rageVal = default;

    public RageValue(T prerageVal, T rageVal)
    {
        this.prerageVal = prerageVal;
        this.rageVal = rageVal;
    }

    public T GetVal(bool rage) => rage ? rageVal : prerageVal;
}

public enum BossSpawnEnum { INDEPENDENT = 0, TERR_HORIZ = 1, TERR_VERT = 2 }
