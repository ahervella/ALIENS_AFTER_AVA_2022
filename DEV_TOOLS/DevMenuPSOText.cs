using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DevMenuPSOText : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI psoName = null;

    [SerializeField]
    private TextMeshProUGUI psoVal = null;

    private PropertySO pso;

    public void SetPSO(PropertySO pso)
    {
        this.pso = pso;
        psoName.text = pso.name;
        pso.DevMenu_SubscribeToPSO(UpdatePSOVal);
        UpdatePSOVal();
    }

    private void UpdatePSOVal()
    {
        psoVal.text = pso.ValueToString();
    }
}
