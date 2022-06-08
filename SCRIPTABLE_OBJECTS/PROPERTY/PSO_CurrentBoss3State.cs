using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PSO_CurrentBoss3State", menuName = "ScriptableObjects/Property/PSO_CurrentBoss3State")]
public class PSO_CurrentBoss3State : PropertySO<Boss3State>
{
    public override void ModifyValue(Boss3State mod)
    {
        if (Value != mod)
        {
            SetValue(mod);
        }
    }
}
