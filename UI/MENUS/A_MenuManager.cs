using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using System;

public abstract class A_MenuManager<T> : MonoBehaviour
{
    [SerializeField]
    private MenuButton selectedStartButton = null;

    [SerializeField]
    public bool disableKeyInputs = false;

    [SerializeField]
    protected SO_InputManager inputManager = null;

    [SerializeField]
    protected MenuButtonGroup<T> buttonGroup = null;

    protected bool MenuEnabled { get; set; } = true;

    private MenuButton selectedButton;
    private MenuButton mouseSelectedButton = null;

    private void Awake()
    {
        buttonGroup.ForEachButton(mb => mb.SetOnMouseSelectMethod(OnMouseSelectButtonChanged));
        buttonGroup.ForEachButton(mb => mb.SetOnMousePressMethod(OnMouseButtonPress));

        if (!disableKeyInputs)
        {
            SelectButton(selectedStartButton);
        }

        OnMenuAwake();
        //TODO: make AssignMenuButtons an abstract class and move in here
    }

    protected abstract void OnMenuAwake();

    private void Start()
    {
        if (!disableKeyInputs)
        {
            inputManager.RegisterForInput(InputEnum.NAV_DIR, InputManager_OnNavDirPressed);
            inputManager.RegisterForInput(InputEnum.NAV_SELECT, InputManager_OnNavSelect);
        }

        OnMenuStart();
    }

    protected abstract void OnMenuStart();

    private void OnMouseSelectButtonChanged(MenuButton button, bool selectedVsDeselected)
    {
        if (!MenuEnabled) { return; }

        if (selectedVsDeselected)
        {
            mouseSelectedButton = button;
            SelectButton(button);
            return;
        }

        mouseSelectedButton = null;
        if (button != selectedButton)
        {
            button.OnDeselect();
        }

    }

    private void SelectButton(MenuButton button)
    {
        if (!MenuEnabled) { return; }

        buttonGroup.ForEachButton(mb => mb.OnDeselect());
        selectedButton = button;
        selectedButton.OnSelect();
        mouseSelectedButton?.OnSelect();
    }

    private void InputManager_OnNavDirPressed(CallbackContext context)
    {
        if (!MenuEnabled) { return; }

        Vector2 val = context.ReadValue<Vector2>();
        MenuButton button = null;

        if (val == Vector2.left)
        {
            button = selectedButton.GetAdjacentButton(ButtonNavEnum.LEFT);
        }
        else if (val == Vector2.right)
        {
            button = selectedButton.GetAdjacentButton(ButtonNavEnum.RIGHT);
        }
        else if (val == Vector2.up)
        {
            button = selectedButton.GetAdjacentButton(ButtonNavEnum.UP);
        }
        else if (val == Vector2.down)
        {
            button = selectedButton.GetAdjacentButton(ButtonNavEnum.DOWN);
        }

        if (button != null)
        {
            SelectButton(button);
        }
    }

    private void OnMouseButtonPress()
    {
        if (!MenuEnabled) { return; }

        mouseSelectedButton.OnPress();
        SelectButton(mouseSelectedButton);
    }

    private void InputManager_OnNavSelect(CallbackContext context)
    {
        if (!MenuEnabled) { return; }

        selectedButton.OnPress();
    }


    protected void AssignOnButtonPressedMethod(T enumVal, Action pressMethod)
    {
        MenuButton bttn = buttonGroup.GetButton(enumVal);
        bttn.SetPressMethod(pressMethod);
    }

    protected void ClearOnButtonPressedMethod(T enumVal)
    {
        buttonGroup.GetButton(enumVal).SetPressMethod(null);
    }
}
