using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DelegateSO<PARAM_TYPE, RETURN_TYPE> : ScriptableObject
{
    //TODO: make this accept multiple methods instead of just one

    [NonSerialized]
    private bool registeredWithGameModeManager = false;

    public delegate RETURN_TYPE InvokeDelegate(PARAM_TYPE param);

    InvokeDelegate OnInvokeDelegateMethod;

    public void SetInvokeMethod(InvokeDelegate invokeMethod, bool persistant = false)
    {
        OnInvokeDelegateMethod = invokeMethod;
        if (!persistant)
        {
            RegisterForGameModeReplaced(invokeMethod);
        }
    }

    private void RegisterForGameModeReplaced(InvokeDelegate invokeMethod)
    {
        if (!registeredWithGameModeManager)
        {
            S_GameModeManager.Current.RegisterForGameModeSceneUnloaded(S_GameModeManager_OnGameModeReplaced);
            registeredWithGameModeManager = true;
        }
    }

    private void S_GameModeManager_OnGameModeReplaced()
    {
        OnInvokeDelegateMethod = null;
    }

    public virtual RETURN_TYPE InvokeDelegateMethod(PARAM_TYPE param)
    {
        if (OnInvokeDelegateMethod == null)
        {
            Debug.Log($"Invoke method for DelegateSO {name} not set!");
            return default;
        }

        return OnInvokeDelegateMethod(param);
    }
}
