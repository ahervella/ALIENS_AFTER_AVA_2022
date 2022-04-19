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
    protected IntPropertySO currZone = null;

    [SerializeField]
    private Vector3PropertySO hitBoxDimEdgePerc = null;

    [SerializeField]
    private BoxColliderSP hitBox = null;

    [SerializeField]
    private AnimationEventExtender removeBossAEExtender = null;

    [SerializeField]
    protected PSO_CurrentZonePhase currZonePhase = null;

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
        hitBox.SetOnTriggerEnterMethod(OnTriggerEnterDamageHitBox);

        removeBossAEExtender.AssignAnimationEvent(AE_OnRemoveBoss, 0);

        SetStartingPosition();

        StartCoroutine(HealthBarSpawnCR());

        OnBossAwake();
    }

    private IEnumerator HealthBarSpawnCR()
    {
        yield return new WaitForSeconds(settings.HealthBarSpawnDelay);
        Instantiate(settings.HealthBarPrefab);
    }

    private void OnTriggerEnterDamageHitBox(Collider other)
    {
        Projectile projectile = other.gameObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.OnEnteredBoss(this);
        }
    }


    private void AE_OnRemoveBoss()
    {
        //TODO: Do elimination sequence for one sprite has fallen?
        //currZone.ModifyValue(1);
        SafeDestroy(gameObject);
    }

    protected abstract void SetStartingPosition();

    protected abstract void OnBossAwake();

    public override void TakeDamage(int damage)
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
            currZonePhase.ModifyValue(ZonePhaseEnum.BOSS_RAGE);
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
