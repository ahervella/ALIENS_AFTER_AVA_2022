using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PSO_CurrentZonePhase", menuName = "ScriptableObjects/Property/PSO_CurrentZonePhase")]
public class PSO_CurrentZonePhase : PropertySO<ZonePhaseEnum>
{
    public override void ModifyValue(ZonePhaseEnum mod)
    {
        if (Value != mod)
        {
            SetValue(mod);
        }
    }
}
