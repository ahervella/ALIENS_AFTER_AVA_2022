using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunner : MonoBehaviour
{
    [SerializeField]
    private SO_InputManager inputManager = null;

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

    private PlayerActionEnum currAction = PlayerActionEnum.RUN;

    private void Awake()
    {
        RegisterForInputs();
        SetPlayerStartPosition();
    }

    private void Start()
    {
        StartAction(PlayerActionEnum.RUN);
    }

    private void SetPlayerStartPosition()
    {
        float x = terrSettings.LaneCount / 2f * terrSettings.TileDims.x;
        float z = settings.StartRowsFromEnd * terrSettings.TileDims.y;
        transform.position = new Vector3(x, 0, z);
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
        if (currAction == PlayerActionEnum.RUN || currAction == PlayerActionEnum.FALL)
        {
            StartAction(PlayerActionEnum.DODGE_L);
        }
    }

    private void InputManager_DodgeRight()
    {
        Debug.Log("Input_DodgeRight");
        if (currAction == PlayerActionEnum.RUN || currAction == PlayerActionEnum.FALL)
        {
            StartAction(PlayerActionEnum.DODGE_R);
        }
    }

    private void InputManager_Jump()
    {
        Debug.Log("Input_Jump");
        if (currAction == PlayerActionEnum.RUN)
        {
            StartAction(PlayerActionEnum.JUMP);
        }

        else if (currAction == PlayerActionEnum.SPRINT)
        {
            StartAction(PlayerActionEnum.LONG_JUMP);
        }
    }

    private void InputManager_Sprint()
    {
        Debug.Log("Input_Sprint");
        if (currAction == PlayerActionEnum.RUN)
        {
            StartAction(PlayerActionEnum.SPRINT);
        }
    }

    private void InputManager_Roll()
    {
        Debug.Log("Input_Roll");
        if (currAction == PlayerActionEnum.RUN)
        {
            StartAction(PlayerActionEnum.ROLL);
        }
    }

    private void InputManager_Pause()
    {
        Debug.Log("Pause");
        //Pause Game
    }

    private void StartAction(PlayerActionEnum action)
    {
        SO_CameraAngle ca = settings.GetActionCameraAngle(action);
        if ( ca != null)
        {
            targetCameraAngle.ModifyValue(ca);
        }

        switch (action)
        {
            case PlayerActionEnum.RUN:
                //For each:
                //StartAnim
                break;
            case PlayerActionEnum.DODGE_L:
                //Also trigger lane change left
                StartCoroutine(StartLaneChange(-1));
                break;
            case PlayerActionEnum.DODGE_R:
                StartCoroutine(StartLaneChange(1));
                //Also trigger lane change right
                break;
            case PlayerActionEnum.FALL:
                break;
            case PlayerActionEnum.JUMP:
                break;
            case PlayerActionEnum.LJ_FALL:
                break;
            case PlayerActionEnum.LONG_JUMP:
                break;
            case PlayerActionEnum.ROLL:
                break;
            case PlayerActionEnum.SPRINT:
                break;
            case PlayerActionEnum.HURT_CENTER:
                break;
            case PlayerActionEnum.HURT_LOWER:
                break;
            case PlayerActionEnum.HURT_UPPER:
                break;
            case PlayerActionEnum.HURT_AIR:
                break;
        }

        currAction = action;
    }

    private IEnumerator StartLaneChange(int dir)
    {
        yield return new WaitForSeconds(settings.LaneChangeDelay);
        laneChange.ModifyValue(new LaneChange(dir, settings.LaneChangeTime));
        yield return new WaitForSeconds(settings.LaneChangeTime);
        StartAction(PlayerActionEnum.RUN);
        //StartAnim
        //ChangeCameraAngle
    }
}


