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
    private FloatPropertySO currEnergy = null;

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

    private void InputManager_DodgeLeft()
    {
        Debug.Log("Input_DodgeLeft");
        if (currAction.Value == PlayerActionEnum.RUN || currAction.Value == PlayerActionEnum.FALL)
        {
            currAction.ModifyValue(PlayerActionEnum.DODGE_L);
        }
    }

    private void InputManager_DodgeRight()
    {
        Debug.Log("Input_DodgeRight");
        if (currAction.Value == PlayerActionEnum.RUN || currAction.Value == PlayerActionEnum.FALL)
        {
            currAction.ModifyValue(PlayerActionEnum.DODGE_R);
        }
    }

    private void InputManager_Jump()
    {
        Debug.Log("Input_Jump");
        if (currAction.Value == PlayerActionEnum.RUN)
        {
            currAction.ModifyValue(PlayerActionEnum.JUMP);
        }

        else if (currAction.Value == PlayerActionEnum.SPRINT)
        {
            currAction.ModifyValue(PlayerActionEnum.LONG_JUMP);
        }
    }

    private void InputManager_Sprint()
    {
        Debug.Log("Input_Sprint");
        if (currAction.Value == PlayerActionEnum.RUN)
        {
            currAction.ModifyValue(PlayerActionEnum.SPRINT);
        }
    }

    private void InputManager_Roll()
    {
        Debug.Log("Input_Roll");
        if (currAction.Value == PlayerActionEnum.RUN)
        {
            currAction.ModifyValue(PlayerActionEnum.ROLL);
        }
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
}


