using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PSO_ArmamentTimerPercent", menuName = "ScriptableObjects/Property/PSO_ArmamentTimerPercent")]
public class PSO_ArmamentTimerPercent : IntPropertySO
{
    protected override int ValueGetter()
    {
        return 100 - currentValue;
    }
}
