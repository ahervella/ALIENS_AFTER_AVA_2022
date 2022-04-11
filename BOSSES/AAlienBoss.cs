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
        this.terrainNode = terrainNode;

        if (settings.SpawnAsChildOfTerr)
        {
            return Instantiate(this, terrainNode.transform);
        }
        return Instantiate(this);
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
