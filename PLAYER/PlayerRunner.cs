using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private PSO_LaneChange laneChange = null;

    [SerializeField]
    private SO_PlayerRunnerSettings settings = null;

    [SerializeField]
    private SO_TerrSettings terrSettings = null;

    //[SerializeField]
    //private PlayerSounds sounds;

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    //[SerializeField]
    //private PSO_CurrentLoadout currLoadout = null;

    //[SerializeField]
    //private PSO_CurrentInventory currentInventory = null;

    [SerializeField]
    private PSO_CurrentEnergy currEnergy = null;

    [SerializeField]
    private IntPropertySO currLurkCode = null;

    private void Awake()
    {
        //currAction.RegisterForPropertyChanged(OnPlayerActionChange);
        RegisterForInputs();
        SetPlayerStartPosition();
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
    }

    //TODO: take out dev testing for health and energy bar from here eventually!
    private void InputManager_DodgeLeft()
    {
        Debug.Log("Input_DodgeLeft");
        currAction.TryPerform(PlayerActionEnum.DODGE_L);
        TakeDamage(PlayerActionEnum.DODGE_L);
        Debug.Log($"health: {currLives.Value}");
    }

    private void InputManager_DodgeRight()
    {
        Debug.Log("Input_DodgeRight");
        currAction.TryPerform(PlayerActionEnum.DODGE_R);
        currLives.ModifyValue(1);
        Debug.Log($"health: {currLives.Value}");
    }

    private void InputManager_Jump()
    {
        Debug.Log("Input_Jump");
        currAction.TryPerform(PlayerActionEnum.JUMP);
        currEnergy.TryConsumeWeaponEnergy(WeaponEnum.PLASMA_PISTOL);
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
        currEnergy.RewardPlayerEnergy(currAction.Value);
    }

    private void InputManager_Pause()
    {
        Debug.Log("Pause");
        //Pause Game
    }

    public void AE_LaneChange(int dir)
    {
        laneChange.ModifyValue(new LaneChange(dir > 0, settings.LaneChangeTime));
    }

    private void OnEnterHazard(PlayerActionEnum avoidAction, PlayerActionEnum takeDownAction, TerrAddon obstacleType)
    {
        if (avoidAction == currAction.Value)
        {
            currEnergy.RewardPlayerEnergy(currAction.Value);
        }
        else if (takeDownAction == currAction.Value)
        {
            //PerformTakeDown(takeDownAction, obstacleType);
        }
        else
        {
            TakeDamage(avoidAction);
        }
    }

    private void TakeDamage(PlayerActionEnum requiredAction)
    {
        //currAction.PerformCorrespondingHurt(requiredAction);
        currLives.ModifyValue(-1);
    }
}


