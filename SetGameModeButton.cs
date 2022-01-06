using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetGameModeButton : MonoBehaviour, IDisposable
{
    [SerializeField]
    private GameModeEnum gameMode = GameModeEnum.menu;
    [SerializeField]
    private GameModeEnumPropertySO gameModeSO = null;

    Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SetPanel);
        gameModeSO.RegisterForPropertyChanged(OnGameModeChanged);
        OnGameModeChanged(GameModeEnum.menu, gameModeSO.Value);
    }

    public void SetPanel()
    {
        gameModeSO.ModifyValue(gameMode);
    }

    void OnGameModeChanged(GameModeEnum previous, GameModeEnum current)
    {
        button.interactable = current != gameMode;
    }

    public void Dispose()
    {
        button.onClick.RemoveListener(SetPanel);
        gameModeSO.DeRegisterForPropertyChanged(OnGameModeChanged);
    }
}
