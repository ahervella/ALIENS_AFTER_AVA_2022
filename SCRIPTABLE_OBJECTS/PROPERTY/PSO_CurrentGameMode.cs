using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PSO_CurrentGameMode", menuName = "ScriptableObjects/Property/PSO_CurrentGameMode", order = 2)]

public class PSO_CurrentGameMode : PropertySO<GameModeEnum>
{
    public override void ModifyValue(GameModeEnum mod)
    {
        SetValue(mod);
    }
}
