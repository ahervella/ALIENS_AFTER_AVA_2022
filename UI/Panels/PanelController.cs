using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    [SerializeField]
    private PanelsEnum panel = PanelsEnum.ALIENS;
    [SerializeField]
    private PanelsEnumPropertySO panelsSO = null;

    GameObject child;

    private void Awake()
    {
        child = transform.GetChild(0).gameObject;
        panelsSO.RegisterForPropertyChanged( OnPanelsEnumChanged );
        OnPanelsEnumChanged( PanelsEnum.ALIENS, panelsSO.Value );
    }

    void OnPanelsEnumChanged( PanelsEnum previous, PanelsEnum current )
    {
        child.SetActive(current == panel);
    }
}
