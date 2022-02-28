using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PSO_LaneChange", menuName = "ScriptableObjects/Delegates/PSO_LaneChange")]
public class PSO_LaneChange : PropertySO<LaneChange>
{
    public override void ModifyValue(LaneChange mod)
    {
        SetValue(mod);
    }
}

