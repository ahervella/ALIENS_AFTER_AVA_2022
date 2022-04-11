using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PSO_CurrentBoss1State", menuName = "ScriptableObjects/StaticData/PSO_CurrentBoss1State")]
public class PSO_CurrentBoss1State : PropertySO<Boss1State>
{
    public override void ModifyValue(Boss1State mod)
    {
        if (Value != mod)
        {
            SetValue(mod);
        }
    }
}
