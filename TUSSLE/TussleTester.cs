using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class TussleTester : MonoBehaviour
{
    [SerializeField]
    private SO_InputManager inputManager = null;

    [SerializeField]
    private BoolDelegateSO tussleInitDelegate = null;

    [SerializeField]
    private BoolDelegateSO tussleResolveDebugDelegate = null;

    private void Start()
    {
        inputManager.RegisterForInput(InputEnum.DEV_1, InputManager_StartDis);
        inputManager.RegisterForInput(InputEnum.DEV_2, InputManager_StartAdv);
        inputManager.RegisterForInput(InputEnum.DEV_3, InputManager_WinTussle);
        inputManager.RegisterForInput(InputEnum.DEV_4, InputManager_LoseTussle);
    }

    private void InputManager_StartDis(CallbackContext ctx)
    {
        tussleInitDelegate.InvokeDelegateMethod(false);
    }

    private void InputManager_StartAdv(CallbackContext ctx)
    {
        tussleInitDelegate.InvokeDelegateMethod(true);
    }

    private void InputManager_WinTussle(CallbackContext ctx)
    {
        tussleResolveDebugDelegate.InvokeDelegateMethod(true);
    }

    private void InputManager_LoseTussle(CallbackContext ctx)
    {
        tussleResolveDebugDelegate.InvokeDelegateMethod(false);
    }
}
