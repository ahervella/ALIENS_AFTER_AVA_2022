using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPanelButton : MonoBehaviour, IDisposable
{
    [SerializeField]
    private PanelsEnum panel = PanelsEnum.ALIENS;
    [SerializeField]
    private PanelsEnumPropertySO panelsSO = null;

    Button button;
    
    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SetPanel);
        panelsSO.RegisterForPropertyChanged(OnPanelsEnumChanged);
        OnPanelsEnumChanged(PanelsEnum.COLLECTABLES, panelsSO.Value);
    }

    public void SetPanel()
    {
        panelsSO.ModifyValue(panel);
    }

    void OnPanelsEnumChanged( PanelsEnum previous, PanelsEnum current )
    {
        button.interactable = current != panel;
    }

    public void Dispose()
    {
        button.onClick.RemoveListener(SetPanel);
        panelsSO.DeRegisterForPropertyChanged(OnPanelsEnumChanged);
    }
}
