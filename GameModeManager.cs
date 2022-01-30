using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeManager : MonoBehaviour, IDisposable
{

    [SerializeField]
    private PSO_CurrentGameMode gameModeSO = null;

    public void Dispose()
    {
        gameModeSO.DeRegisterForPropertyChanged(OnGameModeChanged);
    }

    private void Awake()
    {
        gameModeSO.RegisterForPropertyChanged(OnGameModeChanged);
    }

    void OnGameModeChanged(GameModeEnum previous, GameModeEnum current)
    {
        Time.timeScale = current == GameModeEnum.MENU ? 0 : 1;
    }
}
