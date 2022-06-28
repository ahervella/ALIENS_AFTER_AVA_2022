using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using TMPro;

public class TutorialInputTextManager : MonoBehaviour
{
    [SerializeField]
    private bool disableManager = true;
    
    [SerializeField]
    private BoolPropertySO inputIsKeyboardPSO = null;

    [SerializeField]
    private List<GameObject> keyboardInputObjects = new List<GameObject>();

    [SerializeField]
    private List<GameObject> controllerInputObjects = new List<GameObject>();

    private void Awake()
    {
        if (disableManager) { return; }
        inputIsKeyboardPSO.RegisterForPropertyChanged(InputManager_OnInput);
    }

    private void Start()
    {
        if (disableManager) { return; }
        InputManager_OnInput(inputIsKeyboardPSO.Value, inputIsKeyboardPSO.Value);
    }

    private void InputManager_OnInput(bool _, bool isKeyboardInput)
    {
        foreach (GameObject go in keyboardInputObjects)
        {
            go.SetActive(isKeyboardInput);
        }

        foreach (GameObject go in controllerInputObjects)
        {
            go.SetActive(!isKeyboardInput);
        }
    }

}
