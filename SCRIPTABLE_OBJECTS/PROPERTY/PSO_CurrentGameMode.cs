using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PSO_CurrentGameMode", menuName = "ScriptableObjects/Property/PSO_CurrentGameMode", order = 2)]

public class PSO_CurrentGameMode : PropertySO<GameModeEnum>
{
    private GameModeEnum prevVal;
    public GameModeEnum PrevValue => prevVal;

    public override void ModifyValue(GameModeEnum mod)
    {
        if (Value != mod)
        {
            SetValue(mod);
        }
    }
}
