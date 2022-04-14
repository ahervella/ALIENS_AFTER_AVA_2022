using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

public abstract class AAlienBoss<BOSS_STATE, BOSS_SETTINGS> : AAlienBossBase where BOSS_SETTINGS : SO_ABossSettings
{
    [SerializeField]
    protected BOSS_SETTINGS settings = null;

    [SerializeField]
    protected SO_TerrSettings terrSettings = null;

    [SerializeField]
    protected PropertySO<BOSS_STATE> currState = null;

    [SerializeField]
    private Vector3PropertySO hitBoxDimEdgePerc = null;

    [SerializeField]
    private BoxColliderSP hitBox = null;

    protected bool Rage { get; private set; } = false;

    protected int health;

    //Do we need this if we're setting it before instantiating
    //the prefab?
    [NonSerialized]
    protected EnvTreadmill terrainNode = null;

    public override AAlienBossBase InstantiateBoss(EnvTreadmill terrainNode)
    {
        AAlienBoss<BOSS_STATE, BOSS_SETTINGS> instance = null;
        switch (settings.SpawnType)
        {
            case BossSpawnEnum.INDEPENDENT:
                instance = Instantiate(this);
                break;
            case BossSpawnEnum.TERR_HORIZ:
                instance = Instantiate(this, terrainNode.HorizTransform);
                break;
            case BossSpawnEnum.TERR_VERT:
                instance = Instantiate(this, terrainNode.VertTransform);
                break;
        }

        instance.terrainNode = terrainNode;
        return instance;
    }

    private void Awake()
    {
        health = settings.StartingHealth;

        SetHitBoxDimensions(
            hitBox,
            new Vector2(settings.HitBoxTileWidth, 1),
            terrSettings,
            hitBoxDimEdgePerc);

        SetStartingPosition();
        OnBossAwake();
    }

    protected abstract void SetStartingPosition();

    protected abstract void OnBossAwake();

    protected void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            InitDeath();
        }

        if (health <= settings.RageHealthThreshold)
        {
            Debug.Log($"Activated Rage mode for boss {name}");
            Rage = true;
            InitRage();
        }
    }

    protected abstract void InitDeath();

    protected abstract void InitRage();

    //Health
    //Hitbox
    //Anim
    //Switch to rage mode

}
