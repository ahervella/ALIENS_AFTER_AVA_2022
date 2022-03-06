using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    private Coroutine healCR = null;

    [SerializeField]
    private DSO_LaneChange laneChangeDelegate = null;

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

    private Coroutine sprintCR = null;

    private void Awake()
    {
        RegisterForInputs();
        SetPlayerStartPosition();
        StartHealCoroutine();
        currAction.RegisterForPropertyChanged(OnActionChange);
        shieldOnFlag.RegisterForPropertyChanged(OnShieldChange);
    }

    private void Start()
    {
        currAction.ModifyValue(PlayerActionEnum.RUN);
    }

    private void SetPlayerStartPosition()
    {
        float x = terrSettings.LaneCount / 2f * terrSettings.TileDims.x;
        float z = settings.StartRowsFromEnd * terrSettings.TileDims.y;
        transform.position = new Vector3(x, 0, z) + settings.StartPosOffset;
    }

    private void RegisterForInputs()
    {
        inputManager.RegisterForInput(InputEnum.GAME_LEFT, InputManager_DodgeLeft);
        inputManager.RegisterForInput(InputEnum.GAME_RIGHT, InputManager_DodgeRight);
        inputManager.RegisterForInput(InputEnum.GAME_JUMP, InputManager_Jump);
        inputManager.RegisterForInput(InputEnum.GAME_SPRINT, InputManager_Sprint);
        inputManager.RegisterForInput(InputEnum.GAME_ROLL, InputManager_Roll);
        inputManager.RegisterForInput(InputEnum.GAME_PAUSE, InputManager_Pause);

        inputManager.RegisterForInput(InputEnum.DEV_1, InputManager_Dev1);
        inputManager.RegisterForInput(InputEnum.DEV_2, InputManager_Dev2);
        inputManager.RegisterForInput(InputEnum.DEV_3, InputManager_Dev3);
        inputManager.RegisterForInput(InputEnum.DEV_4, InputManager_Dev4);
        inputManager.RegisterForInput(InputEnum.DEV_5, InputManager_Dev5);
        inputManager.RegisterForInput(InputEnum.DEV_6, InputManager_Dev6);
        inputManager.RegisterForInput(InputEnum.DEV_7, InputManager_Dev7);
        inputManager.RegisterForInput(InputEnum.DEV_8, InputManager_Dev8);
        inputManager.RegisterForInput(InputEnum.DEV_9, InputManager_Dev9);
    }

    //TODO: take out dev testing for health and energy bar from here eventually!
    private void InputManager_DodgeLeft()
    {
        Debug.Log("Input_DodgeLeft");
        currAction.TryPerform(PlayerActionEnum.DODGE_L);
    }

    private void InputManager_DodgeRight()
    {
        Debug.Log("Input_DodgeRight");
        currAction.TryPerform(PlayerActionEnum.DODGE_R);
    }

    private void InputManager_Jump()
    {
        Debug.Log("Input_Jump");
        currAction.TryPerform(PlayerActionEnum.JUMP);
    }

    private void InputManager_Sprint()
    {
        Debug.Log("Input_Sprint");
        currAction.TryPerform(PlayerActionEnum.SPRINT);
    }

    private void InputManager_Roll()
    {
        Debug.Log("Input_Roll");
        currAction.TryPerform(PlayerActionEnum.ROLL);
    }

    private void InputManager_Pause()
    {
        Debug.Log("Pause");
        //Pause Game
    }

    private void InputManager_Dev1()
    {
        TakeDamage(PlayerActionEnum.DODGE_L);
        Debug.Log($"health: {currLives.Value}");
    }

    private void InputManager_Dev2()
    {
        currLives.ModifyValue(1);
        Debug.Log($"health: {currLives.Value}");
    }

    private void InputManager_Dev3()
    {
        useArmament.InvokeDelegateMethod(currLoadout.Value.OrderedWeapons[0]);
    }

    private void InputManager_Dev4()
    {
        //currEnergy.RewardPlayerEnergy(currAction.Value);
        TryStartSprint();
    }

    private bool dev_toggleTreadmill = true;

    private void InputManager_Dev5()
    {
        if (dev_toggleTreadmill)
        {
            AE_PauseTreadmill(1f);
            return;
        }

        AE_ResumeTreadmill(1f);

        dev_toggleTreadmill = !dev_toggleTreadmill;
    }


    private void InputManager_Dev6()
    {
        useArmament.InvokeDelegateMethod(currLoadout.Value.OrderedEquipments[0]);
    }

    private void InputManager_Dev7()
    {
    }

    private void InputManager_Dev8()
    {
    }

    private void InputManager_Dev9()
    {
    }


    private void OnShieldChange(bool oldVal, bool newVal)
    {

    }

    private void OnActionChange(PlayerActionEnum oldAction, PlayerActionEnum newAction)
    {
        StopSprintCR();
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

    public void AE_LaneChange(int dir)
    {
        laneChangeDelegate.InvokeDelegateMethod(new LaneChange(dir > 0, settings.LaneChangeTime));
    }

    public void AE_ResumeTreadmill(float transitionTime)
    {
        treadmillToggleDelegate.InvokeDelegateMethod(new TreadmillSpeedChange(1, transitionTime));
    }

    public void AE_PauseTreadmill(float transitionTime)
    {
        treadmillToggleDelegate.InvokeDelegateMethod(new TreadmillSpeedChange(0, transitionTime));
    }

    public void OnEnterHazard(PlayerActionEnum avoidAction, PlayerActionEnum takeDownAction, TerrAddonEnum obstacleType)
    {
        if (avoidAction == currAction.Value)
        {
            currEnergy.RewardPlayerEnergy(currAction.Value);
            return;
        }

        if (takeDownAction == currAction.Value)
        {
            //PerformTakeDown(takeDownAction, obstacleType);
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

        currAction.PerformCorrespondingHurt(requiredAction);
        currLives.ModifyValue(-1);
    }

    private void StartHealCoroutine()
    {
        if (healCR != null)
        {
            StopCoroutine(healCR);
        }

        healCR = StartCoroutine(HealDamageCoroutine());
    }

    private IEnumerator HealDamageCoroutine()
    {
        while (currLives.Value < currLives.MaxValue())
        {
            yield return new WaitForSeconds(settings.LifeRecoveryTime);
            currLives.ModifyValue(1);
        }

        healCR = null;
    }
}


