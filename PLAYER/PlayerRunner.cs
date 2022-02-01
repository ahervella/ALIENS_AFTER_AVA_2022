using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRunner : MonoBehaviour
{
    [SerializeField]
    private SO_InputManager inputManager;

    //[SerializeField]
    //private PSO_TargetCameraAngle targetCameraAngle;

    //[SerializeField]
    //private List<CameraAngleWrapper> cameraAnglesMap;

    [SerializeField]
    private IntPropertySO currLives;

    [SerializeField]
    private PSO_LaneChange laneChange;

    [SerializeField]
    private SO_PlayerRunnerSettings settings;

    //[SerializeField]
    //private PlayerSounds sounds;

    [SerializeField]
    private PSO_CurrentGameMode currGameMode;

    //[SerializeField]
    //private PSO_CurrentLoadout currLoadout;

    //[SerializeField]
    //private PSO_CurrentInventory currentInventory;

    [SerializeField]
    private FloatPropertySO currEnergy;

    [SerializeField]
    private IntPropertySO currLurkCode;

    private void Awake()
    {
        RegisterForInputs();
    }

    private void RegisterForInputs()
    {
        inputManager.RegisterForInput(InputEnum.GAME_LEFT, InputManager_DodgeLeft);
        inputManager.RegisterForInput(InputEnum.GAME_RIGHT, InputManager_DodgeRight);
        inputManager.RegisterForInput(InputEnum.GAME_JUMP, InputManager_Jump);
        inputManager.RegisterForInput(InputEnum.GAME_SPRINT, InputManager_Sprint);
        inputManager.RegisterForInput(InputEnum.GAME_ROLL, InputManager_Roll);
        inputManager.RegisterForInput(InputEnum.GAME_FIRE_WEAPON, InputManager_FireWeapon);
        inputManager.RegisterForInput(InputEnum.GAME_PREV_WEAPON, InputManager_PrevWeapon);
        inputManager.RegisterForInput(InputEnum.GAME_NEXT_WEAPON, InputManager_NextWeapon);
        inputManager.RegisterForInput(InputEnum.GAME_EQUIPMENT_1, InputManager_Equipment1);
        inputManager.RegisterForInput(InputEnum.GAME_EQUIPMENT_2, InputManager_Equipment2);
        inputManager.RegisterForInput(InputEnum.GAME_PAUSE, InputManager_Pause);
    }

    private void InputManager_DodgeLeft()
    {
        Debug.Log("DodgeLeft");
    }

    private void InputManager_DodgeRight()
    {
        Debug.Log("DodgeRight");
    }

    private void InputManager_Jump()
    {
        Debug.Log("Jump");
    }

    private void InputManager_Sprint()
    {
        Debug.Log("Sprint");
    }

    private void InputManager_Roll()
    {
        Debug.Log("Roll");
    }

    private void InputManager_FireWeapon()
    {
        Debug.Log("FireWeapon");
    }

    private void InputManager_PrevWeapon()
    {
        Debug.Log("PrevWeapon");
    }

    private void InputManager_NextWeapon()
    {
        Debug.Log("NextWeapon");
    }

    private void InputManager_Equipment1()
    {
        Debug.Log("Equipment1");
    }

    private void InputManager_Equipment2()
    {
        Debug.Log("Equipment2");
    }

    private void InputManager_Pause()
    {
        Debug.Log("Pause");
    }
}


