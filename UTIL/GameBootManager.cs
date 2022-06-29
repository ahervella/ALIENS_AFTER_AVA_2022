using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBootManager : MonoBehaviour
{
    [SerializeField]
    private SO_DeveloperToolsSettings devTools = null;

    [SerializeField]
    private BoolPropertySO firstTimePlayingPSO = null;

    [SerializeField]
    private SO_GameSaveManager saveManager = null;

    private void Start()
    {
        // if (devTools.DemoMode)
        // {
        //     firstTimePlayingPSO.ModifyValue(true);
        // }


        bool successful = saveManager.TryLoadGameSave();
        Debug.Log("Loaded save file successful: " + successful);
    }
}
