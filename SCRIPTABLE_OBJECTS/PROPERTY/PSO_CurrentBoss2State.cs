using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PSO_CurrentBoss2State", menuName = "ScriptableObjects/Property/PSO_CurrentBoss2State")]
public class PSO_CurrentBoss2State : PropertySO<Boss2State>
{
    public override void ModifyValue(Boss2State mod)
    {
        if (Value != mod)
        {
            SetValue(mod);
        }
    }
}
