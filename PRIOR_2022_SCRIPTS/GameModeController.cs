using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeController : MonoBehaviour
{
    [SerializeField]
    private GameModeEnum gameMode = GameModeEnum.MENU;
    [SerializeField]
    private GameModeEnumPropertySO gameModeSO = null;

    GameObject child;

    private void Awake()
    {
        child = transform.GetChild(0).gameObject;
        gameModeSO.RegisterForPropertyChanged(OnPanelsEnumChanged);
        OnPanelsEnumChanged(GameModeEnum.MENU, gameModeSO.Value);
    }

    void OnPanelsEnumChanged(GameModeEnum previous, GameModeEnum current)
    {
        child.SetActive(current == gameMode);
    }

}
