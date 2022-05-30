using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
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
    protected Vector3PropertySO hitBoxDimEdgePerc = null;

    [SerializeField]
    protected int hitBoxHeight = 1;

    [SerializeField]
    private BoxColliderSP hitBox = null;
    public override BoxColliderSP HitBox() => hitBox;

    //public PlayerActionEnum GetRequiredAvoidAction(BoxColliderSP hb)
    //{
    //    return HelperUtil.GetRequiredAvoidAction(hitBox, reqActionDict, hb);
    //}

    [SerializeField]
    private AnimationEventExtender removeBossAEExtender = null;

    [SerializeField]
    protected IntPropertySO currHealth = null;

    [SerializeField]
    protected PSO_CurrentZonePhase currZonePhase = null;

    [SerializeField]
    private SO_InputManager inputManager = null;

    [SerializeField]
    private AudioWrapperSource audioSource = null;

    [SerializeField]
    private SpriteFlasher damageFlash = null;

    [SerializeField]
    private BoolDelegateSO bossTussleDamageDSO = null;

    [SerializeField]
    private SO_DamageQuantSettings damageSettings = null;

    private AFillBarManagerBase healthBarPrefab;

    private bool stunned = false;

    protected bool Rage { get; private set; } = false;

    //Assuming all bosses will start with BOSS_SPAWN, and after their spawn
    //sequence will make themselves not invincible
    protected bool invincible = true;

    //TODO: Do we need this if we're setting it before instantiating
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
        inputManager.RegisterForInput(InputEnum.DEV_9, InputManager_Dev9);


        currHealth.RegisterForPropertyChanged(OnHealthChanged);
        currHealth.ModifyValue(settings.StartingHealth - currHealth.Value);

        currZonePhase.RegisterForPropertyChanged(OnZonePhaseChange);

        bossTussleDamageDSO.RegisterForDelegateInvoked(OnBossTussleDamage);

        InitHitBoxes();

        removeBossAEExtender.AssignAnimationEvent(AE_RemoveBoss, 0);

        SetStartingPosition();

        StartCoroutine(HealthBarSpawnCR());

        StartCoroutine(PlaySpawnAudioCR());

        OnBossAwake();
    }

    //Need this so that second boss (or any future boss) can take advantage
    //of multiple hit boxes. Ideally though would rather spend more time making
    //bosses a base class of a terr hazard...
    protected virtual void InitHitBoxes()
    {
        SetHitBoxDimensions(
            hitBox,
            new Vector2(settings.HitBoxTileWidth, 1),
            hitBoxHeight,
            terrSettings,
            hitBoxDimEdgePerc);

        hitBox.SetOnTriggerEnterMethod(
            coll => OnTriggerEnterBossHitBox(coll, hitBox, tussleOnAttack: true));
    }

    private void InputManager_Dev9(CallbackContext ctx)
    {
        InitDeath();
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        if (newHealth <= 0)
        {
            InitDeath();
            currHealth.DeRegisterForPropertyChanged(OnHealthChanged);
            return;
        }

        //in case boss gains health
        if (newHealth >= oldHealth) { return; }

        damageFlash.Flash();

        if (newHealth <= settings.RageHealthThreshold && !Rage)
        {
            Debug.Log($"Activated Rage mode for boss {name}");
            Rage = true;
            currZonePhase.ModifyValue(ZonePhaseEnum.BOSS_RAGE);
            InitRage();
        }
        else
        {
            settings.HurtAudioWrapper?.PlayAudioWrapper(audioSource);
        }
    }


    

    private void OnZonePhaseChange(ZonePhaseEnum oldPhase, ZonePhaseEnum newPhase)
    {
        if (newPhase == ZonePhaseEnum.BOSS)
        {
            invincible = false;
        }
    }

    private int OnBossTussleDamage(bool isForBoss)
    {
        if (isForBoss)
        {
            TakeDamage(damageSettings.GetTussleDamage(damage2PlayerOrAlien: false));
        }
        return 0;
    }

    private IEnumerator HealthBarSpawnCR()
    {
        yield return new WaitForSeconds(settings.HealthBarSpawnDelay);
        healthBarPrefab = Instantiate(settings.HealthBarPrefab);
    }

    private IEnumerator PlaySpawnAudioCR()
    {
        yield return new WaitForSeconds(settings.SpawnAudioDelay);
        settings.SpawnAudioWrapper?.PlayAudioWrapper(audioSource);
    }

    public override void Stun()
    {
        stunned = true;
    }

    protected void OnTriggerEnterBossHitBox(Collider other, BoxColliderSP hbSource, bool tussleOnAttack)
    {
        BoxColliderSP hb = other.gameObject.GetComponent<BoxColliderSP>();
        if (hb?.RootParent == hbSource) { return; }

        Projectile projectile = hb?.RootParent?.GetComponent<Projectile>();

        if (projectile != null)
        {
            projectile.OnEnteredBoss(this);
            return;
        }


        PlayerRunner player = hb?.RootParent?.GetComponent<PlayerRunner>();
        if (player != null)
        {
            player.OnEnterHazard(
                hbSource.RequiredAvoidAction,
                stunned ? PlayerActionEnum.ANY_ACTION : PlayerActionEnum.NULL,
                TerrAddonEnum.BOSS,
                tussleOnAttack,
                out bool _,
                out bool _
                );

            stunned = false;
        }

    }


    protected void AE_RemoveBoss()
    {
        currHealth.DeRegisterForPropertyChanged(OnHealthChanged);
        currZonePhase.DeRegisterForPropertyChanged(OnZonePhaseChange);

        //TODO: Do elimination sequence for one sprite has fallen?
        healthBarPrefab.TearDown(settings.TearDownDelayPostDeath);

        currZonePhase.ModifyValue(ZonePhaseEnum.ZONE_TRANS);

        ExtraRemoveBoss();

        SafeDestroy(gameObject);
    }


    protected abstract void SetStartingPosition();

    protected abstract void OnBossAwake();

    public override void TakeDamage(int damage)
    {
        if (invincible) { return; }
        currHealth.ModifyValue(-damage);
    }

    protected abstract void InitDeath();

    protected abstract void ExtraRemoveBoss();

    protected abstract void InitRage();

    //Health
    //Hitbox
    //Anim
    //Switch to rage mode

}
