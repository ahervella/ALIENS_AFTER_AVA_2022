using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    public delegate void OnInput();

    public void RegisterForInput(InputEnum input, OnInput method)
    {
        //idealy we find somewhere to do this on awake but
        //this is the only way to make sure that anything that
        //subscribes has the controls enabled.
        if (!inputMapper.enabled)
        {
            inputMapper.Enable();
        }

        foreach (InputWrapper iw in inputWrappers)
        {
            if (iw.InputType == input)
            {
                iw.InputAction.action.performed -= ctx => method();
                iw.InputAction.action.performed += ctx => method();
                return;
            }
        }

        Debug.LogErrorFormat("Couldn't find InputWrapper for inputEnum {0}", input.ToString());
    }


}


