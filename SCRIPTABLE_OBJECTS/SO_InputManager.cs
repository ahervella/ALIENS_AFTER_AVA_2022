using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

[CreateAssetMenu(fileName = "SO_InputManager", menuName = "ScriptableObjects/StaticData/SO_InputManager")]
public class SO_InputManager : ScriptableObject
{
    [SerializeField]
    private InputActionAsset inputMapper;

    [SerializeField]
    private List<InputWrapper> inputWrappers = new List<InputWrapper>();

    [Serializable]
    private class InputWrapper
    {
        [SerializeField]
        private InputActionReference inputAction;
        public InputActionReference InputAction => inputAction;

        [SerializeField]
        private InputEnum inputType;
        public InputEnum InputType => inputType;
    }

    [NonSerialized]
    private bool registeredWithGameModeManager = false;

    [NonSerialized]
    private Dictionary<InputWrapper, Action<CallbackContext>>
        unregisterOnGameModeSceneReplaceDict = new Dictionary<InputWrapper, Action<CallbackContext>>();

    public void RegisterForAnyInput(Action<CallbackContext> method, bool persistant = false)
    {
        AnyInputRegistrationChanged(method, true, persistant);
    }

    public void UnregisterFromAnyInput(Action<CallbackContext> method, bool persistant = false)
    {
        AnyInputRegistrationChanged(method, false, persistant);
    }

    private void AnyInputRegistrationChanged(Action<CallbackContext> method, bool registering, bool persistant)
    {
        if (registering)
        {
            EnsureIsEnabled();
        }

        foreach (InputWrapper iw in inputWrappers)
        {
            iw.InputAction.action.performed -= method;
            if (registering)
            {
                iw.InputAction.action.performed += method;
            }

            RegisterForGameModeSceneUnloaded(iw, method, registering, persistant);
        }
    }

    public void EnsureIsEnabled()
    {
        //idealy we find somewhere to do this on awake but
        //this is the only way to make sure that anything that
        //subscribes has the controls enabled.
        if (!inputMapper.enabled)
        {
            inputMapper.Enable();
        }
    }


    public void RegisterForInput(InputEnum input, Action<CallbackContext> method, bool persistant = false)
    {
        InputRegistrationChanged(input, method, true, persistant);
    }

    public void UnregisterFromInput(InputEnum input, Action<CallbackContext> method, bool persistant = false)
    {
        InputRegistrationChanged(input, method, false, persistant);
    }

    private void InputRegistrationChanged(InputEnum input, Action<CallbackContext> method, bool registering, bool persistant)
    {
        if (registering)
        {
            EnsureIsEnabled();
        }

        foreach (InputWrapper iw in inputWrappers)
        {
            if (iw.InputType == input)
            {
                iw.InputAction.action.performed -= method;
                if (registering)
                {
                    iw.InputAction.action.performed += method;
                }

                RegisterForGameModeSceneUnloaded(iw, method, registering, persistant);

                return;
            }
        }

        Debug.LogErrorFormat("Couldn't find InputWrapper for inputEnum {0}", input.ToString());
    }

    private void RegisterForGameModeSceneUnloaded(InputWrapper iw, Action<CallbackContext> method, bool registering, bool persistant)
    {
        if (!registeredWithGameModeManager)
        {
            S_GameModeManager.Current.RegisterForGameModeSceneUnloaded(S_GameModeManager_OnGameModeSceneUnloaded);
            registeredWithGameModeManager = true;
        }

        //TODO any way to make this logic cleaner?
        if (!unregisterOnGameModeSceneReplaceDict.ContainsKey(iw))
        {
            if (!registering || persistant) { return; }
            unregisterOnGameModeSceneReplaceDict.Add(iw, null);
        }

        unregisterOnGameModeSceneReplaceDict[iw] -= method;

        if (!persistant && registering)
        {
            unregisterOnGameModeSceneReplaceDict[iw] += method;
        }
    }

    private void S_GameModeManager_OnGameModeSceneUnloaded()
    {
        foreach(KeyValuePair<InputWrapper, Action<CallbackContext>> kvp in unregisterOnGameModeSceneReplaceDict)
        {
            kvp.Key.InputAction.action.performed -= kvp.Value;
        }
        unregisterOnGameModeSceneReplaceDict.Clear();
    }
}


