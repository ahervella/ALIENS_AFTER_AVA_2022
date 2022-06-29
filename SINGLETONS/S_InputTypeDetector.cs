using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class S_InputTypeDetector : Singleton<S_InputTypeDetector>
{
    [SerializeField]
    private SO_InputManager inputManager = null;

    [SerializeField]
    private BoolPropertySO inputIsKeyboardPSO = null;

    protected override void OnAwake()
    {
        inputManager.RegisterForAnyInput(InputManager_OnInput, persistent = true);
    }

    private void InputManager_OnInput(CallbackContext ctx)
    {
        bool isKeyboardInput = string.Equals(ctx.action.activeControl.device.name, "Keyboard");
        if (inputIsKeyboardPSO.Value != isKeyboardInput)
        {
            Debug.Log("input is keyboard: " + isKeyboardInput);
            inputIsKeyboardPSO.ModifyValue(isKeyboardInput);
        }
    }
}
