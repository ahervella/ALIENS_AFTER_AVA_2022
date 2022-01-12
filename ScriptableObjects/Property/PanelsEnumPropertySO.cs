using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_PanelsEnumPropertySO", menuName = "ScriptableObjects/Property/UI/PanelsEnumPropertySO", order = 2)]
public class PanelsEnumPropertySO : PropertySO<PanelsEnum>
{
    public override void ModifyValue(PanelsEnum mod)
    {
        SetValue(mod);
    }
}
