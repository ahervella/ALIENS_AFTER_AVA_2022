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
        ChangeGameMode(mod, false);
    }

    public void ForceChangeGameMode(GameModeEnum mod)
    {
        ChangeGameMode(mod, true);
    }

    private void ChangeGameMode(GameModeEnum mod, bool forceChange)
    {
        if (Value != mod || forceChange)
        {
            prevVal = Value;
            Debug.Log("Changing game mode to " + mod.ToString());
            SetValue(mod);

            if (mod == GameModeEnum.QUIT)
            {
                Debug.Log("Quitting game...");
                S_GameModeManager.Current.QuitGame();
                return;
            }

            S_GameModeManager.Current.TryReplaceGameModeScene(mod);
        }
    }
}
