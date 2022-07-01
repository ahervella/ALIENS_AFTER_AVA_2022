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
    private string devInputMapName = string.Empty;

    [SerializeField]
    private string gameInputActionMapName = string.Empty;

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

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

    //Need the last dictionary party because turns out the InputAction.action.performed has
    //overriden add and remove Callback functionality that allows to add the same methods with
    //different targets (aka from different inherited scripts) to the action,
    //which is inaccessible due to being part of the dll. This is the work around
    [NonSerialized]
    private Dictionary<InputWrapper, Dictionary<object, Action<CallbackContext>>>
        unregisterOnGameModeSceneReplaceDict
        = new Dictionary<InputWrapper, Dictionary<object, Action<CallbackContext>>>();

    private void OnEnable()
    {
        inputMapper.Enable();

        if (Application.isEditor)
        {
            inputMapper.FindActionMap(devInputMapName).Disable();
        }

        currGameMode.RegisterForPropertyChanged(OnGameModeChange);
    }

    private void OnGameModeChange(GameModeEnum oldMode, GameModeEnum newMode)
    {
        if (newMode == GameModeEnum.PAUSE)
        {
            inputMapper.FindActionMap(gameInputActionMapName).Disable();
        }

        else if (oldMode == GameModeEnum.PAUSE && newMode == GameModeEnum.PLAY)
        {
            inputMapper.FindActionMap(gameInputActionMapName).Enable();
        }
    }
    
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
            unregisterOnGameModeSceneReplaceDict.Add(iw, new Dictionary<object, Action<CallbackContext>>());
        }

        if (!unregisterOnGameModeSceneReplaceDict[iw].ContainsKey(method.Target))
        {
            unregisterOnGameModeSceneReplaceDict[iw].Add(method.Target, null);
        }

        unregisterOnGameModeSceneReplaceDict[iw][method.Target] -= method;

        if (!persistant && registering)
        {
            unregisterOnGameModeSceneReplaceDict[iw][method.Target] += method;
        }
    }

    private void S_GameModeManager_OnGameModeSceneUnloaded()
    {
        foreach(KeyValuePair<InputWrapper, Dictionary<object, Action<CallbackContext>>> kvp in unregisterOnGameModeSceneReplaceDict)
        {

            foreach(KeyValuePair < object, Action<CallbackContext>> method_kvp in unregisterOnGameModeSceneReplaceDict[kvp.Key])
            {
                kvp.Key.InputAction.action.performed -= method_kvp.Value;
            }
        }

        unregisterOnGameModeSceneReplaceDict.Clear();
    }
}


