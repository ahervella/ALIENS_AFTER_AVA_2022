using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[CreateAssetMenu(fileName = "SO_GameSaveManager", menuName = "ScriptableObjects/StaticData/SO_GameSaveManager")]
public class SO_GameSaveManager : ScriptableObject
{
    [SerializeField]
    private BoolPropertySO firstTimePlayingPSO = null;

    [SerializeField]
    private BoolPropertySO grappleLockedPSO = null;

    [SerializeField]
    private BoolPropertySO atomCannonLockedPSO = null;

    [SerializeField]
    private BoolPropertySO lvlSelectLockedPSO = null;

    [SerializeField]
    private IntPropertySO currZone = null;

    private string savePath => Application.persistentDataPath + "/autoSave.json";

    private GameSave GetCurrentPSOGameState()
    {
        return new GameSave(
            firstTimePlayingPSO,
            grappleLockedPSO,
            atomCannonLockedPSO,
            lvlSelectLockedPSO,
            currZone
        );
    }

    private void LoadGameSave2PSOs(GameSave gs)
    {
        firstTimePlayingPSO.LoadSaveState(gs.FirstTimePlaying);
        grappleLockedPSO.LoadSaveState(gs.GrappleLocked);
        atomCannonLockedPSO.LoadSaveState(gs.AtomCannonLocked);
        lvlSelectLockedPSO.LoadSaveState(gs.LvlSelectLocked);
        currZone.LoadSaveState(gs.Zone);
    }

    [Serializable]
    private class GameSave
    {
        public bool FirstTimePlaying;
        public bool GrappleLocked;
        public bool AtomCannonLocked;
        public bool LvlSelectLocked;
        public int Zone;

        public GameSave(
            BoolPropertySO firstTimePlayingPSO,
            BoolPropertySO grappleLockedPSO,
            BoolPropertySO atomCannonLockedPSO,
            BoolPropertySO lvlSelectLockedPSO,
            IntPropertySO currZone
        )
        {
            FirstTimePlaying = firstTimePlayingPSO.Value;
            GrappleLocked = grappleLockedPSO.Value;
            AtomCannonLocked = atomCannonLockedPSO.Value;
            LvlSelectLocked = lvlSelectLockedPSO.Value;
            Zone = currZone.Value;
        }
    }


    public void SaveGameState()
    {
        string fileContents = JsonUtility.ToJson(GetCurrentPSOGameState());
        File.WriteAllText(savePath, fileContents);
    }

    public bool TryLoadGameSave()
    {
        if (!File.Exists(savePath)) { return false; }
        
        string fileContents = File.ReadAllText(savePath);
        GameSave loadedGameSave = JsonUtility.FromJson<GameSave>(fileContents);//FromJsonOverwrite(fileContents, loadedGameSave);

        LoadGameSave2PSOs(loadedGameSave);
        return true;
    }
}
