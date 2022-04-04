using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

[RequireComponent(typeof(Button))]
public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField]
    private MenuButtonGroup<ButtonNavEnum> adjacentButtons = null;

    [SerializeField]
    private TextMeshProUGUI text = null;

    [SerializeField]
    private Color idleColor = default;

    [SerializeField]
    private Color selectColor = default;

    //The action paramaters are this button (MenuButton)
    //and whether the mouse entered or exited (bool)
    private Action<MenuButton, bool> OnMouseSelectButtonChangedMethod = null;
    private Action OnMousePressButtonMethod = null;

    private Action OnPressMethod = null;

    public MenuButton GetAdjacentButton(ButtonNavEnum dir)
    {
        return adjacentButtons.GetButton(dir);
    }

    public void SetOnMouseSelectMethod(Action<MenuButton, bool> selectMethod)
    {
        OnMouseSelectButtonChangedMethod = selectMethod;
    }

    public void SetOnMousePressMethod(Action pressMethod)
    {
        OnMousePressButtonMethod = pressMethod;
    }

    public void SetPressMethod(Action method)
    {
        OnPressMethod = method;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseSelectButtonChangedMethod(this, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseSelectButtonChangedMethod(this, false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnMousePressButtonMethod();
    }

    public void OnSelect()
    {
        text.color = selectColor;
        text.fontStyle = FontStyles.Underline | FontStyles.Bold;
    }

    public void OnDeselect()
    {
        text.color = idleColor;
        text.fontStyle = FontStyles.Bold;
    }

    public void OnPress()
    {
        Debug.Log("pressed " + name);
        OnPressMethod?.Invoke();
    }

    public void SetAlpha(float a)
    {
        //Debug.Log("alpha set to " + a);
        text.color = new Color(text.color.r, text.color.g, text.color.b, a);
    }
}

public enum ButtonNavEnum
{
    LEFT = 0, RIGHT = 1, UP = 2, DOWN = 3
}

