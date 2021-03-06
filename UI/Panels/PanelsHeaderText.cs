using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelsHeaderText : MonoBehaviour, IDisposable
{
    [SerializeField]
    private PanelsEnumPropertySO panelsSO = null;

    private Text text;

    private void Start()
    {
        text = GetComponent<Text>();
        panelsSO.RegisterForPropertyChanged(OnPanelsSOChanged);
        OnPanelsSOChanged(PanelsEnum.ALIENS, panelsSO.Value);
    }

    void OnPanelsSOChanged(PanelsEnum previous, PanelsEnum current)
    {
        text.text = current.ToString();
    }

    public void Dispose()
    {
        panelsSO.DeRegisterForPropertyChanged(OnPanelsSOChanged);
    }

}

