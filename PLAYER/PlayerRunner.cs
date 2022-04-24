using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;
using static HelperUtil;

[RequireComponent(typeof(BoxCollider))]
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

    //[SerializeField]
    //private PlayerSounds sounds;

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    [SerializeField]
    private PSO_CurrentLoadout currLoadout = null;

    //[SerializeField]
    //private PSO_CurrentInventory currentInventory = null;

    [SerializeField]
    private PSO_CurrentEnergy currEnergy = null;

    [SerializeField]
    private IntPropertySO currLurkCode = null;

    [SerializeField]
    private BoolPropertySO shieldOnFlag = null;

    [SerializeField]
    private BoolDelegateSO tussleInitDelegate = null;

    [SerializeField]
    private BoolPropertySO currEndOfDemo = null;

    [SerializeField]
    private SO_DeveloperToolsSettings developerSettings = null;

    [SerializeField]
    private SO_EnergySettings energySettings = null;

    private Coroutine sprintCR = null;

    private void Awake()
    {
        SetPlayerStartPosition();
        currAction.RegisterForPropertyChanged(OnActionChange);
        shieldOnFlag.RegisterForPropertyChanged(OnShieldChange);
        currGameMode.RegisterForPropertyChanged(OnGameModeChange);
        currEndOfDemo.RegisterForPropertyChanged(OnDemoEnd);
    }

    private void Start()
    {
        RegisterForInputs();
        currAction.ModifyValue(PlayerActionEnum.RUN);
    }

    private void SetPlayerStartPosition()
    {
        float x = GetLaneXPosition(0, terrSettings);
        float z = settings.StartRowsFromEnd * terrSettings.TileDims.y;
        transform.position = new Vector3(x, 0, z) + settings.StartPosOffset;
        fadeSettings.InitFadeSettings(transform.position);
    }

    private void RegisterForInputs()
    {
        inputManager.RegisterForInput(InputEnum.GAME_LEFT, InputManager_DodgeLeft);
        inputManager.RegisterForInput(InputEnum.GAME_RIGHT, InputManager_DodgeRight);
        inputManager.RegisterForInput(InputEnum.GAME_JUMP, InputManager_Jump);
        inputManager.RegisterForInput(InputEnum.GAME_SPRINT, InputManager_Sprint);
        inputManager.RegisterForInput(InputEnum.GAME_ROLL, InputManager_Roll);
        //inputManager.RegisterForInput(InputEnum.GAME_PAUSE, InputManager_Pause);

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
        //inputManager.UnregisterFromInput(InputEnum.GAME_PAUSE, InputManager_Pause);

        inputManager.UnregisterFromInput(InputEnum.DEV_1, InputManager_Dev1);
        inputManager.UnregisterFromInput(InputEnum.DEV_2, InputManager_Dev2);
        inputManager.UnregisterFromInput(InputEnum.DEV_3, InputManager_Dev3);
        inputManager.UnregisterFromInput(InputEnum.DEV_4, InputManager_Dev4);
        inputManager.UnregisterFromInput(InputEnum.DEV_5, InputManager_Dev5);
        inputManager.UnregisterFromInput(InputEnum.DEV_6, InputManager_Dev6);
        inputManager.UnregisterFromInput(InputEnum.DEV_7, InputManager_Dev7);
        inputManager.UnregisterFromInput(InputEnum.DEV_8, InputManager_Dev8);
    }

    private void OnDemoEnd(bool prevVal, bool newVal)
    {
        if (!newVal) { return; }
        UnregisterFromInputs();
    }

    //TODO: take out dev testing for health and energy bar from here eventually!
    private void InputManager_DodgeLeft(CallbackContext ctx)
    {
        Debug.Log("Input_DodgeLeft");
        currAction.TryPerform(PlayerActionEnum.DODGE_L);
    }

    private void InputManager_DodgeRight(CallbackContext ctx)
    {
        Debug.Log("Input_DodgeRight");
        currAction.TryPerform(PlayerActionEnum.DODGE_R);
    }

    private void InputManager_Jump(CallbackContext ctx)
    {
        Debug.Log("Input_Jump");
        currAction.TryPerform(PlayerActionEnum.JUMP);
    }

    private void InputManager_Sprint(CallbackContext ctx)
    {
        Debug.Log("Input_Sprint");
        //currAction.TryPerform(PlayerActionEnum.SPRINT);
    }

    private void InputManager_Roll(CallbackContext ctx)
    {
        Debug.Log("Input_Roll");
        currAction.TryPerform(PlayerActionEnum.ROLL);
    }

    private void InputManager_Dev1(CallbackContext ctx)
    {
        //TakeDamage(PlayerActionEnum.DODGE_L);
        //Debug.Log($"health: {currLives.Value}");
    }

    private void InputManager_Dev2(CallbackContext ctx)
    {
        currLives.ModifyValue(1);
    }

    private void InputManager_Dev3(CallbackContext ctx)
    {
        TryUseArmament(true, 0);
    }

    private void InputManager_Dev4(CallbackContext ctx)
    {
        //currEnergy.RewardPlayerEnergy(currAction.Value);
        //TryStartSprint();
    }

    private void InputManager_Dev5(CallbackContext ctx)
    {
    }


    private void InputManager_Dev6(CallbackContext ctx)
    {
        TryUseArmament(false, 0);
    }

    private void InputManager_Dev7(CallbackContext ctx)
    {
        TryUseArmament(false, 1);
    }

    private void InputManager_Dev8(CallbackContext ctx)
    {
    }


    private void OnShieldChange(bool oldVal, bool newVal)
    {

    }

    private void OnActionChange(PlayerActionEnum oldAction, PlayerActionEnum newAction)
    {
        StopSprintCR();
    }

    private void OnGameModeChange(GameModeEnum oldMode, GameModeEnum newMode)
    {
        //return;
        if (newMode == GameModeEnum.PAUSE)
        {
            UnregisterFromInputs();
        }
        if (oldMode == GameModeEnum.PAUSE && newMode == GameModeEnum.PLAY)
        {
            RegisterForInputs();
        }
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
        if (!currAction.TryPerform(PlayerActionEnum.SPRINT)) { return false; }
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
        if (!currAction.IsPlayingHurtAnim())
        {
            currEnergy.ModifyValue(energySettings.GetEnergyReward(currAction.Value));
        }
    }

    public void OnEnterHazard(
        PlayerActionEnum avoidAction,
        PlayerActionEnum takeDownAction,
        TerrAddonEnum obstacleType,
        out bool dodged)
    {
        dodged = false;

        //TODO: since grappling puts alien in stun, do we need the first check?
        //Is it safe to leave it in case there is an alien infront of the one we grappled??
        //Even though that shouldn't ever be possible (unless an alien pops out in front?)
        if (/*currAction.Value == PlayerActionEnum.GRAPPLE_REEL
            || */takeDownAction == currAction.Value
            || takeDownAction == PlayerActionEnum.ANY_ACTION)
        {
            StartTussle(true);
            return;
        }

        if (avoidAction == currAction.Value)
        {
            currEnergy.RewardPlayerEnergy(currAction.Value);
            dodged = true;
            return;
        }

        if (shieldOnFlag.Value)
        {
            //Only break the shield if it wasn't a projectile
            if (obstacleType != TerrAddonEnum.PROJECTILE)
            {
                shieldOnFlag.ModifyValue(false);
            }
            return;
        }

        TakeDamage(avoidAction);
    }

    public void OnEnterProjectile(WeaponEnum weaponType, PlayerActionEnum avoidAction, out bool dodged)
    {
        //TODO: do we need tyhe projectile (weapon) type in the end?
        OnEnterHazard(PlayerActionEnum.ROLL, avoidAction, TerrAddonEnum.PROJECTILE, out dodged);
    }

    private void StartTussle(bool advantage)
    {
        Debug.Log("Loading tussle scene...");
        currAction.ModifyValue(PlayerActionEnum.TUSSLE);
        tussleInitDelegate.InvokeDelegateMethod(advantage);
    }

    private void TakeDamage(PlayerActionEnum requiredAction)
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
        if (developerSettings.Invincibility || currEndOfDemo.Value) { return; }

        //TODO: seperate sounds of player getting hurt due to hurt action,
        //and impact of specific objects
        currAction.PerformCorrespondingHurt(requiredAction);
        currLives.ModifyValue(-1);
    }

    

    private bool TryUseArmament(bool isWeapon, int loadoutIndex)
    {
        if (currAction.IsPlayingHurtAnim()) { return false; }

        if (isWeapon)
        {
            useArmament.InvokeDelegateMethod(currLoadout.Value.OrderedWeapons[loadoutIndex]);
        }
        else
        {
            useArmament.InvokeDelegateMethod(currLoadout.Value.OrderedEquipments[loadoutIndex]);
        }

        return true;
    }
}


