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
    private Vector3PropertySO hitBoxDimEdgePerc = null;

    [SerializeField]
    private BoxColliderSP hitBox = null;

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
    private AlienBossDamageFlash damageFlash = null;

    private AFillBarManagerBase healthBarPrefab;

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

        SetHitBoxDimensions(
            hitBox,
            new Vector2(settings.HitBoxTileWidth, 1),
            terrSettings,
            hitBoxDimEdgePerc);
        hitBox.SetOnTriggerEnterMethod(OnTriggerEnterDamageHitBox);

        removeBossAEExtender.AssignAnimationEvent(AE_OnRemoveBoss, 0);

        SetStartingPosition();

        StartCoroutine(HealthBarSpawnCR());

        StartCoroutine(PlaySpawnAudioCR());

        OnBossAwake();
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

        damageFlash.FlashWhiteDamage();

        if (newHealth <= settings.RageHealthThreshold && !Rage)
        {
            Debug.Log($"Activated Rage mode for boss {name}");
            Rage = true;
            currZonePhase.ModifyValue(ZonePhaseEnum.BOSS_RAGE);
            InitRage();
        }
        else
        {
            settings.HurtAudioWrapper.PlayAudioWrapper(audioSource);
        }
    }


    

    private void OnZonePhaseChange(ZonePhaseEnum oldPhase, ZonePhaseEnum newPhase)
    {
        if (newPhase == ZonePhaseEnum.BOSS)
        {
            invincible = false;
        }
    }

    private IEnumerator HealthBarSpawnCR()
    {
        yield return new WaitForSeconds(settings.HealthBarSpawnDelay);
        healthBarPrefab = Instantiate(settings.HealthBarPrefab);
    }

    private IEnumerator PlaySpawnAudioCR()
    {
        yield return new WaitForSeconds(settings.SpawnAudioDelay);
        settings.SpawnAudioWrapper.PlayAudioWrapper(audioSource);
    }

    private void OnTriggerEnterDamageHitBox(Collider other)
    {
        Projectile projectile = other.transform.parent.gameObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.OnEnteredBoss(this);
        }
    }


    private void AE_OnRemoveBoss()
    {
        currHealth.DeRegisterForPropertyChanged(OnHealthChanged);
        currZonePhase.DeRegisterForPropertyChanged(OnZonePhaseChange);

        //TODO: Do elimination sequence for one sprite has fallen?
        healthBarPrefab.TearDown(settings.TearDownDelayPostDeath);

        currZonePhase.ModifyValue(ZonePhaseEnum.NO_BOSS_SUB1);
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

    protected abstract void InitRage();

    //Health
    //Hitbox
    //Anim
    //Switch to rage mode

}
