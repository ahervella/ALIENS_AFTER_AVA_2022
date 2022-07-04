using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PSO_CurrentZonePhase", menuName = "ScriptableObjects/Property/PSO_CurrentZonePhase")]
public class PSO_CurrentZonePhase : PropertySO<ZonePhaseEnum>
{
    [SerializeField]
    private IntPropertySO currZone = null;

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    [SerializeField]
    private PSO_CurrentTutorialMode currTutMode = null;

    [SerializeField]
    private SO_TerrZoneWrapperSettings zoneWrapperSettings = null;

    [SerializeField]
    private SO_GameSaveManager saveManager = null;

    [SerializeField]
    private GameObject fade2BlackPrefab = null;

    [SerializeField]
    private float fade2TutorialTime = 1f;

    [SerializeField]
    private SO_DeveloperToolsSettings devTools = null;

    //TODO: Make a universal location to hold the
    //black screen time when changing scenes with the game mode manager
    [SerializeField]
    private float fade2TutorialHoldBlackTime = 1.5f;

    [SerializeField]
    private SO_SteamManager steamManager = null;

    public override void ModifyValue(ZonePhaseEnum mod)
    {
        if (Value != mod)
        {
            if (Value == ZonePhaseEnum.END_OF_ZONE)
            {
                SO_TerrZoneWrapper zw = zoneWrapperSettings.GetZoneWrapper(currZone.Value);

                TryLoadTutorial(zw);
                TryCompleteAchievement();
                currZone.ModifyValue(1);
                zw.TutorialOneShotPSO.ModifyValue(false);
                saveManager.SaveGameState();
            }
            SetValue(mod);
        }
    }

    private void TryLoadTutorial(SO_TerrZoneWrapper zw)
    {
        if (devTools.SkipTutorials) { return; }
        
        if (zw.TutorialOnFinish != TutorialModeEnum.NONE && zw.TutorialOneShotPSO.Value)
        {
            GameObject inst = Instantiate(fade2BlackPrefab);
            FadeToBlack ftb = inst.GetComponent<FadeToBlack>();
            ftb.InitFade(
                fadeInOrOut: false,
                fade2TutorialTime,
                delay: 0,
                fade2TutorialHoldBlackTime,
                () => GoToTutorialScene(zw));
        }
    }

    private void TryCompleteAchievement()
    {
        SO_TerrZoneWrapper zw = zoneWrapperSettings.GetZoneWrapper(currZone.Value);
        SteamAchievementsEnum ach = zw.GetSteamAchForZonePhase(Value);

        if (ach == SteamAchievementsEnum.NONE) { return; }

        steamManager.TryCompleteAchievement(ach);
    }

    private void GoToTutorialScene(SO_TerrZoneWrapper zw)
    {
            currTutMode.ModifyValue(zw.TutorialOnFinish);
            currGameMode.ModifyValue(GameModeEnum.TUTORIAL);
    }
}
