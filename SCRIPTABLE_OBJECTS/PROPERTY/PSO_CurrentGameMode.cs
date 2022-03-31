using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PSO_CurrentGameMode", menuName = "ScriptableObjects/Property/PSO_CurrentGameMode", order = 2)]

public class PSO_CurrentGameMode : PropertySO<GameModeEnum>
{
    [NonSerialized]
    private GameModeEnum prevVal = GameModeEnum.BOOT;
    public GameModeEnum PrevValue => prevVal;

    public override void ModifyValue(GameModeEnum mod)
    {
        if (Value != mod)
        {
            prevVal = Value;
            Debug.Log("Changing game mode to " + mod.ToString());
            SetValue(mod);
            S_GameModeManager.Current.TryReplaceGameModeScene(mod);
        }
    }
}
