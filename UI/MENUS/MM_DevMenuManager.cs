using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;
using Tayx.Graphy;
using static UnityEngine.InputSystem.InputAction;

public class MM_DevMenuManager : A_MenuManager<DevMenuButtonEnum>
{
    [SerializeField]
    private SO_DeveloperToolsSettings settings = null;

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    [SerializeField]
    private RectTransform menuContainer = null;

    [SerializeField]
    private RectTransform psoListSpawnRef = null;

    [SerializeField]
    private DevMenuPSOText psoTextPrefab = null;

    [SerializeField]
    private TextMeshProUGUI versionText = null;

    [SerializeField]
    private BoolPropertySO devMenuVisible = null;

    [SerializeField]
    private SO_GameSaveManager saveManager = null;

    protected override void OnMenuAwake()
    {
        MakeAllPSOTexts();
        inputManager.RegisterForInput(InputEnum.DEV_MENU, OnDevMenuToggle, true);
        AssignOnButtonPressedMethod(DevMenuButtonEnum.TOGGLE_FULL_PSO, OnToggleFullPSOList);
        AssignOnButtonPressedMethod(DevMenuButtonEnum.LOAD_RUN, () => OnButtonLoadGameMode(GameModeEnum.PLAY));
        AssignOnButtonPressedMethod(DevMenuButtonEnum.LOAD_BOSS, () => OnLoadBoss());
        AssignOnButtonPressedMethod(DevMenuButtonEnum.LOAD_MAINMENU, () => OnButtonLoadGameMode(GameModeEnum.MAINMENU));
        AssignOnButtonPressedMethod(DevMenuButtonEnum.LOAD_TUTORIAL, () => OnButtonLoadGameMode(GameModeEnum.TUTORIAL));
        AssignOnButtonPressedMethod(DevMenuButtonEnum.LOAD_BOOT, () => OnButtonLoadGameMode(GameModeEnum.BOOT));


        foreach (DevMenuButtonEnum dmb in (DevMenuButtonEnum[])Enum.GetValues(typeof(DevMenuButtonEnum)))
        {
            if ((int)dmb >= 100) { continue; }
            buttonGroup.GetButton(dmb).SetText(settings.TryGetModName(dmb) ?? "ERROR_NO_NAME_FOUND");
            AssignOnButtonPressedMethod(dmb, () => settings.TrySetMod(dmb));
        }

        versionText.text = settings.CurrBuildVersion;
    }

    protected override void OnMenuStart()
    {
    }

    private void MakeAllPSOTexts()
    {
        //In case the prefab already had stuff I was testing (or if we ever
        //call this twice for whatever)
        for(int i = 0; i < psoListSpawnRef.childCount; i++)
        {
            Destroy(psoListSpawnRef.GetChild(i).gameObject);
        }

        foreach(PropertySO pso in settings.ExposedPSOs)
        {
            DevMenuPSOText instance = Instantiate(psoTextPrefab, psoListSpawnRef);
            instance.SetPSO(pso);
        }
    }

    private void OnButtonLoadGameMode(GameModeEnum gameMode, bool bossStart = false)
    {
        Debug.Log("LOADING GAMEMODE: " + gameMode);
        settings.SetSpawnBossOnStart(bossStart);
        currGameMode.ForceChangeGameMode(gameMode);
    }

    private void OnDevMenuToggle(CallbackContext ctx)
    {
        menuContainer.gameObject.SetActive(!menuContainer.gameObject.activeSelf);
        if (menuContainer.gameObject.activeSelf)
        {
            GraphyManager.Instance.Enable();
            devMenuVisible.ModifyValue(true);
            inputManager.RegisterForInput(InputEnum.DEV_8, InputManager_SaveGameState);
            inputManager.RegisterForInput(InputEnum.DEV_9, InputManager_LoadGameSave);
        }
        else
        {
            GraphyManager.Instance.Disable();
            devMenuVisible.ModifyValue(false);
            inputManager.UnregisterFromInput(InputEnum.DEV_8, InputManager_SaveGameState);
            inputManager.UnregisterFromInput(InputEnum.DEV_9, InputManager_LoadGameSave);
        }
    }

    private void InputManager_SaveGameState(CallbackContext ctx)
    {
        saveManager.SaveGameState();
        Debug.Log("Saved to:" + Application.persistentDataPath);
    }

    private void InputManager_LoadGameSave(CallbackContext ctx)
    {
        bool successful = saveManager.TryLoadGameSave();
        Debug.Log("Loading save file: " + (successful ? "success" : "failure"));
    }

    private void OnToggleFullPSOList()
    {
        psoListSpawnRef.gameObject.SetActive(!psoListSpawnRef.gameObject.activeSelf);
    }

    private void OnLoadBoss()
    {
        OnButtonLoadGameMode(GameModeEnum.PLAY, true);
    }
}

//treadmill start speed delta
//treadmill acceleration delta
//treadmill fog distance delta
//treamill tile dims delta
//camera FOV delta
//spawn all enemies and shit at once, instead of sequentally
//starting lives
//invincibility
//spawn boss immediately
//

public enum DevMenuButtonEnum
{
    TOGGLE_FULL_PSO = 100,
    LOAD_MAINMENU = 200,
    LOAD_RUN = 201,
    LOAD_TUTORIAL = 202,
    LOAD_BOOT = 203,
    LOAD_BOSS = 204,
    //All mods need to be between 0 and 99
    //To assign them a button correctly
    TERR_MODE1 = 0,
    TERR_MODE2 = 1,
    TERR_MODE3 = 2,
    TERR_MODE4 = 3,
    BOSS1_MODE1 = 11,
    BOSS1_MODE2 = 12,
    BOSS1_MODE3 = 13
}
