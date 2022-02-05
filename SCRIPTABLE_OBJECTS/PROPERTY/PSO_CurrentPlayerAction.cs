using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PSO_CurrentPlayerAction", menuName = "ScriptableObjects/Property/PSO_CurrentPlayerAction")]
public class PSO_CurrentPlayerAction : PropertySO<PlayerActionEnum>
{
    public override void ModifyValue(PlayerActionEnum mod)
    {
        if (Value != mod)
        {
            SetValue(mod);
        }
    }
}
