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


    public void RegisterForAnyInput(Action<CallbackContext> method)
    {
        AnyInputRegistrationChanged(method, true);
    }

    public void UnregisterFromAnyInput(Action<CallbackContext> method)
    {
        AnyInputRegistrationChanged(method, false);
    }

    private void AnyInputRegistrationChanged(Action<CallbackContext> method, bool registering)
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


    public void RegisterForInput(InputEnum input, Action<CallbackContext> method)
    {
        InputRegistrationChanged(input, method, true);
    }

    public void UnregisterFromInput(InputEnum input, Action<CallbackContext> method)
    {
        InputRegistrationChanged(input, method, false);
    }

    private void InputRegistrationChanged(InputEnum input, Action<CallbackContext> method, bool registering)
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
                return;
            }
        }

        Debug.LogErrorFormat("Couldn't find InputWrapper for inputEnum {0}", input.ToString());
    }
}


