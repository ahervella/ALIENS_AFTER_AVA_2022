using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;
using static HelperUtil;
using System;

public class PlayerRunner : MonoBehaviour
{
    [SerializeField]
    private SO_InputManager inputManager = null;

    [SerializeField]
    private PSO_CurrentPlayerAction currAction = null;
    
    [SerializeField]
    private PSO_TargetCameraAngle targetCameraAngle = null;

    [SerializeField]
    private IntPropertySO currLives = null;

    [SerializeField]
    private DSO_UseArmament useArmament = null;

    [SerializeField]
    private DSO_TreadmillSpeedChange treadmillToggleDelegate = null;

    [SerializeField]
    private PlayerAnimation playerAnimmator = null;

    [SerializeField]
    private SO_PlayerRunnerSettings settings = null;

    [SerializeField]
    private SO_TerrSettings terrSettings = null;

    [SerializeField]
    private SO_TerrNodeFadeSettings fadeSettings = null;

    [SerializeField]
    private SO_TussleSettings tussleSettings = null;

    [SerializeField]
    private PSO_CurrentLoadout currLoadout = null;

    [SerializeField]
    private PSO_CurrentZonePhase currZonePhase = null;

    [SerializeField]
    private DSO_TerrainChange terrainChangeDelegate = null;

    //[SerializeField]
    //private PSO_CurrentInventory currentInventory = null;

    [SerializeField]
    private PSO_CurrentEnergy currEnergy = null;

    [SerializeField]
    private IntPropertySO currLurkCode = null;

    [SerializeField]
    private BoolPropertySO shieldOnFlag = null;

    [SerializeField]
    private PSO_CurrentTussle currTussle = null;

    [SerializeField]
    private SO_DeveloperToolsSettings developerSettings = null;
    

    [SerializeField]
    private SO_LayerSettings layerSettings = null;

    [SerializeField]
    private PSO_TerrainTreadmillNodes terrNodes = null;

    [SerializeField]
    private BoxColliderSP hitBox = null;

    [SerializeField]
    private DSO_LaneChange laneChangeDelegate = null;

    [SerializeField]
    private SO_DamageQuantSettings damageSettings = null;

    [SerializeField]
    private SpriteFlasher damageWhiteFlasher = null;

    private Coroutine sprintCR = null;

    private bool pausedControls = false;

    private Coroutine laneChangeCR = null;
    private float cachedStartXPos;

    private bool tempDodgeInvincibility = false;

    private bool zoneTransitionInvincibility = false;

    private bool damageInvincibility = false;

    private bool Invincible => tempDodgeInvincibility
    || zoneTransitionInvincibility
    || damageInvincibility
    || developerSettings.Invincibility;

    private bool rollDodgedOnce = false;

    private bool jumpDodgedOnce = false;

    private bool rollCanceled = false;

    private bool jumpCanceled = false;

    private void Start()
    {
        SetPlayerStartPosition();
        currAction.RegisterForPropertyChanged(OnActionChange);
        shieldOnFlag.RegisterForPropertyChanged(OnShieldChange);
        currZonePhase.RegisterForPropertyChanged(OnZonePhaseChange);

        tussleSettings.TussleHazardCleanUpDelegate.RegisterForDelegateInvoked(
            OnTussleHazardCleanUpInvoked);

        laneChangeDelegate.RegisterForDelegateInvoked(OnLaneChangeDelegate);

        RegisterForInputs();
        currAction.ModifyValue(PlayerActionEnum.RUN);

        //terrNodes.Value.AttachTransform(hitBox.transform, horizOrVert: true);
        ApplyHitBoxSizeErrorFix(hitBox, towardsOrAwayFromPlayer: true);
    }

    private int OnLaneChangeDelegate(LaneChange lc)
    {
        //SafeStartCoroutine(ref laneChangeCR, LaneChangeCR(lc.Time, lc.Dir), this);
        return 0;
    }

    private IEnumerator LaneChangeCR(float time, int dir)
    {
        PositionChange(hitBox.transform, dir * terrSettings.TileDims.x, 0, 0);
        yield return new WaitForSeconds(time);
        hitBox.transform.position =
            new Vector3(cachedStartXPos, hitBox.transform.position.y, hitBox.transform.position.z);
    }

    private void SetPlayerStartPosition()
    {
        //TODO: set to standard hit box sizes
        float x = GetLaneXPosition(0, terrSettings);
        float z = settings.StartRowsFromEnd * terrSettings.TileDims.y;
        transform.position = new Vector3(x, 0, z) + settings.StartPosOffset;

        cachedStartXPos = transform.position.x;

        fadeSettings.InitFadeSettings(transform.position);
    }

    private void RegisterForInputs()
    {
        inputManager.RegisterForInput(InputEnum.GAME_LEFT, InputManager_DodgeLeft);
        inputManager.RegisterForInput(InputEnum.GAME_RIGHT, InputManager_DodgeRight);
        inputManager.RegisterForInput(InputEnum.GAME_JUMP, InputManager_Jump);
        inputManager.RegisterForInput(InputEnum.GAME_SPRINT, InputManager_Sprint);
        inputManager.RegisterForInput(InputEnum.GAME_ROLL, InputManager_Roll);

        inputManager.RegisterForInput(InputEnum.GAME_FIRE_WEAPON_1, InputManager_Weapon1);
        inputManager.RegisterForInput(InputEnum.GAME_FIRE_WEAPON_2, InputManager_Weapon2);
        inputManager.RegisterForInput(InputEnum.GAME_EQUIPMENT_1, InputManager_Equipment1);
        inputManager.RegisterForInput(InputEnum.GAME_EQUIPMENT_2, InputManager_Equipment2);

        inputManager.RegisterForInput(InputEnum.DEV_1, InputManager_Dev1);
        inputManager.RegisterForInput(InputEnum.DEV_2, InputManager_Dev2);
        inputManager.RegisterForInput(InputEnum.DEV_3, InputManager_Dev3);
        inputManager.RegisterForInput(InputEnum.DEV_4, InputManager_Dev4);
        inputManager.RegisterForInput(InputEnum.DEV_5, InputManager_Dev5);
        inputManager.RegisterForInput(InputEnum.DEV_6, InputManager_Dev6);
        inputManager.RegisterForInput(InputEnum.DEV_7, InputManager_Dev7);
        inputManager.RegisterForInput(InputEnum.DEV_8, InputManager_Dev8);
    }

    private void UnregisterFromInputs()
    {
        inputManager.UnregisterFromInput(InputEnum.GAME_LEFT, InputManager_DodgeLeft);
        inputManager.UnregisterFromInput(InputEnum.GAME_RIGHT, InputManager_DodgeRight);
        inputManager.UnregisterFromInput(InputEnum.GAME_JUMP, InputManager_Jump);
        inputManager.UnregisterFromInput(InputEnum.GAME_SPRINT, InputManager_Sprint);
        inputManager.UnregisterFromInput(InputEnum.GAME_ROLL, InputManager_Roll);

        inputManager.UnregisterFromInput(InputEnum.GAME_FIRE_WEAPON_1, InputManager_Weapon1);
        inputManager.UnregisterFromInput(InputEnum.GAME_FIRE_WEAPON_2, InputManager_Weapon2);
        inputManager.UnregisterFromInput(InputEnum.GAME_EQUIPMENT_1, InputManager_Equipment1);
        inputManager.UnregisterFromInput(InputEnum.GAME_EQUIPMENT_2, InputManager_Equipment2);

        inputManager.UnregisterFromInput(InputEnum.DEV_1, InputManager_Dev1);
        inputManager.UnregisterFromInput(InputEnum.DEV_2, InputManager_Dev2);
        inputManager.UnregisterFromInput(InputEnum.DEV_3, InputManager_Dev3);
        inputManager.UnregisterFromInput(InputEnum.DEV_4, InputManager_Dev4);
        inputManager.UnregisterFromInput(InputEnum.DEV_5, InputManager_Dev5);
        inputManager.UnregisterFromInput(InputEnum.DEV_6, InputManager_Dev6);
        inputManager.UnregisterFromInput(InputEnum.DEV_7, InputManager_Dev7);
        inputManager.UnregisterFromInput(InputEnum.DEV_8, InputManager_Dev8);
    }

    private bool TryPerformAction(PlayerActionEnum action, bool overrideFlag = false)
    {
        Debug.Log("InputAttempt " + action.ToString());
        if (pausedControls) { return false; }
        return currAction.TryPerform(action, playerAnimmator.PrematureActionChangeAllowed || overrideFlag);
    }

    //TODO: take out dev testing for health and energy bar from here eventually!
    private void InputManager_DodgeLeft(CallbackContext ctx)
    {
        if (TryPerformAction(PlayerActionEnum.DODGE_L))
        {
            tempDodgeInvincibility = true;
        }
        else
        {
            TryJumpOrRollDodge(false);
        }
    }

    private void InputManager_DodgeRight(CallbackContext ctx)
    {
        if (TryPerformAction(PlayerActionEnum.DODGE_R))
        {
            tempDodgeInvincibility = true;
        }
        else
        {
            TryJumpOrRollDodge(true);
        }
    }

    private bool TryJumpOrRollDodge(bool movingRightOrLeft)
    {
        if (!rollDodgedOnce && currAction.Value == PlayerActionEnum.ROLL)
        {
            laneChangeDelegate.InvokeDelegateMethod(new LaneChange(movingRightOrLeft, settings.LaneChangeTime));
            rollDodgedOnce = true;
            return true;
        }
        
        if (!jumpDodgedOnce && currAction.Value == PlayerActionEnum.JUMP)
        {
            laneChangeDelegate.InvokeDelegateMethod(new LaneChange(movingRightOrLeft, settings.LaneChangeTime));
            jumpDodgedOnce = true;
            return true;
        }

        return false;
    }

    private void InputManager_Jump(CallbackContext ctx)
    {
        if  (TryPerformAction(PlayerActionEnum.JUMP))
        {
            return;
        }

        bool overrideFlag = !rollCanceled && !jumpCanceled && currAction.Value == PlayerActionEnum.ROLL;
        if (overrideFlag)
        {
            rollCanceled = true;
        }

        TryPerformAction(PlayerActionEnum.JUMP, overrideFlag);
    }

    private void InputManager_Sprint(CallbackContext ctx)
    {
        //TryPerformAction(PlayerActionEnum.SPRINT);
    }

    private void InputManager_Roll(CallbackContext ctx)
    {
        if  (TryPerformAction(PlayerActionEnum.ROLL))
        {
            return;
        }

        bool overrideFlag = !rollCanceled && !jumpCanceled && currAction.Value == PlayerActionEnum.JUMP;
        if (overrideFlag)
        {
            jumpCanceled = true;
        }

        TryPerformAction(PlayerActionEnum.ROLL, overrideFlag);
    }

    private void InputManager_Weapon1(CallbackContext ctx)
    {
        TryUseArmament(true, 0);
    }

    private void InputManager_Weapon2(CallbackContext ctx)
    {
        TryUseArmament(true, 1);
    }

    private void InputManager_Equipment1(CallbackContext ctx)
    {
        TryUseArmament(false, 0);
    }

    private void InputManager_Equipment2(CallbackContext ctx)
    {
        TryUseArmament(false, 1);
    }

    private void InputManager_Dev1(CallbackContext ctx)
    {
    }

    private void InputManager_Dev2(CallbackContext ctx)
    {
    }

    private void InputManager_Dev3(CallbackContext ctx)
    {
    }

    private void InputManager_Dev4(CallbackContext ctx)
    {
    }

    private void InputManager_Dev5(CallbackContext ctx)
    {
    }


    private void InputManager_Dev6(CallbackContext ctx)
    {
    }

    private void InputManager_Dev7(CallbackContext ctx)
    {
        
    }

    private void InputManager_Dev8(CallbackContext ctx)
    {
    }


    private void OnShieldChange(bool oldVal, bool newVal)
    {

    }

    private void OnActionChange(PlayerActionEnum oldAction, PlayerActionEnum newAction)
    {
        tempDodgeInvincibility = false;
        
        if (newAction != PlayerActionEnum.JUMP && newAction != PlayerActionEnum.ROLL)
        {
            rollCanceled = false;
            jumpCanceled = false;
        }

        if (newAction != PlayerActionEnum.ROLL)
        {
            rollDodgedOnce = false;
        }
        
        if (newAction != PlayerActionEnum.JUMP)
        {
            jumpDodgedOnce = false;
        }

        if (currAction.IsHurtAnim(oldAction))
        {
            StartCoroutine(DamageInvincibilityCR());
        }

        StopSprintCR();
    }

    private IEnumerator DamageInvincibilityCR()
    {
        yield return new WaitForSeconds(settings.PostHurtInvincibilityTime);
        damageWhiteFlasher.StopInfFlashing();
        damageInvincibility = false;
    }
    
    private void StopSprintCR()
    {
        if (sprintCR != null)
        {
            StopCoroutine(sprintCR);
        }
    }

    private bool TryStartSprint()
    {
        //Will stop any current sprint cr cause of OnActionChange
        if (!currAction.TryPerform(PlayerActionEnum.SPRINT, playerAnimmator.PrematureActionChangeAllowed)) { return false; }
        sprintCR = StartCoroutine(SprintCoroutine());
        return true;
    }

    private IEnumerator SprintCoroutine()
    {
        yield return new WaitForSeconds(settings.SprintTime);
        playerAnimmator.AE_OnAnimFinished();
        sprintCR = null;
    }

    public void OnExitHazardRewardArea()
    {
        //Reward boxes are only to trigger dodging reward at right time
        if (currAction.Value == PlayerActionEnum.DODGE_L ||
            currAction.Value == PlayerActionEnum.DODGE_R)
        {
            RewardPlayerEnergy();
        }
    }

    public void RewardPlayerEnergy()
    {
        currEnergy.RewardPlayerEnergy(currAction.Value);
    }

    public void OnEnterHazard(
        PlayerActionEnum avoidAction,
        PlayerActionEnum takeDownAction,
        TerrAddonEnum obstacleType,
        bool tussleOnAttack,
        out bool dodged,
        out bool destroyHazard,
        WeaponEnum weaponType = WeaponEnum.NONE)
    {
        dodged = true;
        destroyHazard = false;

        if (currAction.IsPlayingHurtAnim())
        {
            return;
        }


        if (takeDownAction == currAction.Value
            || takeDownAction == PlayerActionEnum.ANY_ACTION)
        {
            StartTussle(true, obstacleType == TerrAddonEnum.BOSS);
            return;
        }

        if (avoidAction == currAction.Value)
        {
            RewardPlayerEnergy();
            return;
        }

        //reeling in means your jumping over stuff
        if (avoidAction == PlayerActionEnum.JUMP
            && currAction.Value == PlayerActionEnum.GRAPPLE_REEL)
        {
            return;
        }

        if (shieldOnFlag.Value)
        {
            //TODO: explode enemies on hit shield
            //Only break the shield if it wasn't a projectile
            if (obstacleType != TerrAddonEnum.PROJECTILE)
            {
                shieldOnFlag.ModifyValue(false);
                destroyHazard = true;
            }

            dodged = false;
            return;
        }

        if (tussleOnAttack)
        {
            if (Invincible) { return; }
            StartTussle(false, obstacleType == TerrAddonEnum.BOSS);
            dodged = false;
            return;
        }

        dodged = !TryTakeDamage(avoidAction, weaponType);
    }

    public void OnEnterProjectile(
        WeaponEnum weaponType,
        PlayerActionEnum avoidAction,
        out bool dodged,
        out bool destroyProjectile)
    {
        //TODO: do we need the projectile (weapon) type in the end?
        OnEnterHazard(avoidAction,
            PlayerActionEnum.NONE,
            TerrAddonEnum.PROJECTILE,
            false,
            out dodged,
            out destroyProjectile,
            weaponType);
    }

    private void StartTussle(bool advantage, bool bossTussle)
    {
        Debug.Log("Loading tussle scene...");
        currAction.ModifyValue(PlayerActionEnum.TUSSLE);
        currTussle.ModifyValue(new TussleWrapper(advantage, bossTussle));
    }

    private int OnTussleHazardCleanUpInvoked(bool _)
    {
        Vector3 raycastPos = new Vector3(transform.position.x, terrSettings.FloorHeight / 2, transform.position.z);       
        float dist = tussleSettings.TussleHazardCleanUpTileDist * terrSettings.TileDims.y;

        int maskLayer = 1 << layerSettings.HitBoxLayer;

        RaycastHit[] hits = Physics.RaycastAll(raycastPos, Vector3.forward, dist, maskLayer);

        foreach (RaycastHit hit in hits)
        {
            BoxColliderSP hitBox = hit.collider.gameObject.GetComponent<BoxColliderSP>();

            if (hitBox != null
                && !(hitBox.RootParent.GetComponent<AAlienBossBase>() is AAlienBossBase))
            {
                SafeDestroy(hitBox.RootParent.gameObject);
            }
        }
        return 0;
    }

    //TODO: move to animation script
    //TODO: reverse the hitbox changes and just work with this?
    public void AE_DodgeInvincibilityOff()
    {
        tempDodgeInvincibility = false;
    }

    private bool TryTakeDamage(PlayerActionEnum requiredAction, WeaponEnum weaponType = WeaponEnum.NONE)
    {
        /*
        //TODO: only necessary if we ever do a mid air hurt with a falling animation
        if (requiredAction == PlayerActionEnum.TAKE_DAMAGE)
        {
            //TODO: make a hurt in mid air?
            //In case we are jumping and run into a tree
            requiredAction = currAction.Value == PlayerActionEnum.JUMP || currAction.Value == PlayerActionEnum.FALLING ?
                PlayerActionEnum.TAKE_DAMAGE_AIR : PlayerActionEnum.TAKE_DAMAGE_GROUND;
        }
        */
        if (Invincible)
        {
            return false;
        }

        //TODO: seperate sounds of player getting hurt due to hurt action,
        //and impact of specific objects
        currAction.PerformCorrespondingHurt(requiredAction);
        damageInvincibility = true;
        damageWhiteFlasher.Flash(FlashType.INF_FLASH_LOOP);

        int damage = weaponType == WeaponEnum.NONE ?
            damageSettings.GetDefaultHazardDamage()
            : damageSettings.GetWeaponDamage(weaponType, damage2PlayerOrAlien: true);

        currLives.ModifyValue(-1 * damage);
        return true;
    }

    

    private bool TryUseArmament(bool isWeapon, int loadoutIndex)
    {
        if (pausedControls) { return false; }
        if (currAction.IsPlayingHurtAnim()) { return false; }

        if (isWeapon)
        {
            return useArmament.InvokeDelegateMethod(currLoadout.Value.OrderedWeapons[loadoutIndex].Armament);
        }
        else
        {
            return useArmament.InvokeDelegateMethod(currLoadout.Value.OrderedEquipments[loadoutIndex].Armament);
        }
    }

    private void OnZonePhaseChange(ZonePhaseEnum _, ZonePhaseEnum newPhase)
    {
        if (newPhase == ZonePhaseEnum.BOSS_SPAWN)
        {
            terrainChangeDelegate.RegisterForDelegateInvoked(OnTerrainChangeDelegate);
        }

        if (newPhase == ZonePhaseEnum.BOSS)
        {
            pausedControls = false;
        }

        zoneTransitionInvincibility = newPhase == ZonePhaseEnum.END_OF_ZONE;
    }

    private int OnTerrainChangeDelegate(TerrainChangeWrapper tcw)
    {
        terrainChangeDelegate.DeRegisterFromDelegateInvoked(OnTerrainChangeDelegate);
        pausedControls = true;
        return 0;
    }
}


