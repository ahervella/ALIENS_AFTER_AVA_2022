using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MenuButton : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IDeselectHandler, IPointerExitHandler
{
    [SerializeField]
    private Button button = null;

    [SerializeField]
    private TextMeshProUGUI text = null;

    [SerializeField]
    private Color idleColor = default;

    [SerializeField]
    private Color selectColor = default;

    public bool ButtonEnabled { get; set; } = true;

    private void Awake()
    {
        OnDeselect();
    }

    public void SubscribeToClick(UnityAction method)
    {
        button.onClick.AddListener(OnClickMethod(method));
    }

    private UnityAction OnClickMethod(UnityAction method)
    {
        if (!ButtonEnabled) { return null; }
        return method;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnSelect(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnDeselect(eventData);
    }

    public void OnSelect(BaseEventData eventData)
    {
        OnSelect();
    }

    public void OnSelect()
    {
        if (!ButtonEnabled) { return; }
        text.color = selectColor;
        text.fontStyle = FontStyles.Underline | FontStyles.Bold;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        OnDeselect();
    }

    public void OnDeselect()
    {
        if (!ButtonEnabled) { return; }
        text.color = idleColor;
        text.fontStyle = FontStyles.Bold;
    }

    public void SetAlpha(float a)
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, a);
    }
}
