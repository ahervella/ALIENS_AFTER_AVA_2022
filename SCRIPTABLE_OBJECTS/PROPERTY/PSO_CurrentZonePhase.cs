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

    public override void ModifyValue(ZonePhaseEnum mod)
    {
        if (Value != mod)
        {
            if (Value == ZonePhaseEnum.END_OF_ZONE)
            {
                TryLoadTutorial();
                currZone.ModifyValue(1);
                saveManager.SaveGameState();
            }
            SetValue(mod);
        }
    }

    private void TryLoadTutorial()
    {
        SO_TerrZoneWrapper zw = zoneWrapperSettings.GetZoneWrapper(currZone.Value);
        if (zw.TutorialOnFinish != TutorialModeEnum.NONE && zw.TutorialOneShotPSO.Value)
        {
            GameObject inst = Instantiate(fade2BlackPrefab);
            FadeToBlack ftb = inst.GetComponent<FadeToBlack>();
            ftb.InitFade(fadeInOrOut: false, fade2TutorialTime, 0, () => GoToTutorialScene(zw));
        }
    }

    private void GoToTutorialScene(SO_TerrZoneWrapper zw)
    {
            zw.TutorialOneShotPSO.ModifyValue(false);
            currTutMode.ModifyValue(zw.TutorialOnFinish);
            currGameMode.ModifyValue(GameModeEnum.TUTORIAL);
    }
}
