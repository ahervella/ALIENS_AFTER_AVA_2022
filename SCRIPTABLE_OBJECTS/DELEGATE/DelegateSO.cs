using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DelegateSO<PARAM_TYPE, RETURN_TYPE> : ScriptableObject
{
    //TODO: base class with property SO

    [NonSerialized]
    private bool registeredWithGameModeManager = false;

    public delegate RETURN_TYPE DelegateInvoked(PARAM_TYPE param);

    DelegateInvoked OnDelegateInvoked;
    DelegateInvoked OnGameModeSceneUnloadedPersistence;

    public void RegisterForDelegateInvoked(DelegateInvoked method, bool persistant = false)
    {
        if (!registeredWithGameModeManager)
        {
            S_GameModeManager.Current.RegisterForGameModeSceneUnloaded(S_GameModeManager_OnGameModeSceneUnloaded);
            registeredWithGameModeManager = true;
        }

        OnDelegateInvoked -= method;
        OnDelegateInvoked += method;

        if (persistant)
        {
            RegisterForGameModeSceneUnloaded(method);
        }
    }

    private void S_GameModeManager_OnGameModeSceneUnloaded()
    {
        OnDelegateInvoked = null;
        if (OnGameModeSceneUnloadedPersistence != null)
        {
            OnDelegateInvoked = OnGameModeSceneUnloadedPersistence;
            return;
        }

        OnGameModeSceneUnloadedPersistence = null;
    }

    private void RegisterForGameModeSceneUnloaded(DelegateInvoked method)
    {
        OnGameModeSceneUnloadedPersistence -= method;
        OnGameModeSceneUnloadedPersistence += method;
    }

    public void DeRegisterFromDelegateInvoked(DelegateInvoked method)
    {
        OnDelegateInvoked -= method;
        OnGameModeSceneUnloadedPersistence -= method;
    }

    public virtual RETURN_TYPE InvokeDelegateMethod(PARAM_TYPE param)
    {
        if (OnDelegateInvoked == null)
        {
            Debug.Log($"No invoke methods for DelegateSO {name} were set!");
            return default;
        }

        //TODO: what happens if we are returning multiple things from here? Does
        //it just get the last or first one?
        return OnDelegateInvoked(param);
    }
}
