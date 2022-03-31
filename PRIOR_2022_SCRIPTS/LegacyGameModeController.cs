using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegacyGameModeController : MonoBehaviour
{
    [SerializeField]
    private GameModeEnum gameMode = GameModeEnum.BACKPACK;
    [SerializeField]
    private PSO_CurrentGameMode gameModeSO = null;

    GameObject child;

    private void Awake()
    {
        child = transform.GetChild(0).gameObject;
        gameModeSO.RegisterForPropertyChanged(OnPanelsEnumChanged);
        OnPanelsEnumChanged(GameModeEnum.BACKPACK, gameModeSO.Value);
    }

    void OnPanelsEnumChanged(GameModeEnum previous, GameModeEnum current)
    {
        child.SetActive(current == gameMode);
    }

}
