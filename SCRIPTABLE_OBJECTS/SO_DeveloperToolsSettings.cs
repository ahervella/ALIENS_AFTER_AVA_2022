using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static HelperUtil;

[CreateAssetMenu(fileName = "SO_DeveloperToolsSettings", menuName = "ScriptableObjects/StaticData/SO_DeveloperToolsSettings")]
public class SO_DeveloperToolsSettings : ScriptableObject
{
    [SerializeField]
    private string currBuildVersion = string.Empty;
    public string CurrBuildVersion => currBuildVersion;

    [SerializeField]
    private bool invincibility = false;
    public bool Invincibility => invincibility;

    [SerializeField]
    private bool instantMainMenuIntro = false;
    public bool InstantMainMenuIntro => instantMainMenuIntro;

    [SerializeField]
    private bool skipDeathSequence = false;
    public bool SkipDeathSequence => skipDeathSequence;

    [SerializeField]
    private bool startWith1Life = false;
    public bool StartWith1Life => startWith1Life;

    [SerializeField]
    private bool spawnOnlyFoley = false;
    public bool SpawnOnlyFoley => spawnOnlyFoley;

    [SerializeField]
    private bool spawnNoTerrAddons = false;
    public bool SpawnNoTerrAddons => spawnNoTerrAddons;

    [SerializeField]
    private bool spawnBossOnStart = false;
    public bool SpawnBossOnStart => spawnBossOnStart;

    [SerializeField]
    private bool demoMode = false;
    public bool DemoMode => demoMode;

    [SerializeField]
    private Loadout firstLoadout = null;
    public Loadout FirstLoadout => firstLoadout;

    [SerializeField]
    private List<DEV_TerrMods> terrMods = new List<DEV_TerrMods>();

    public DEV_TerrMods currTerrMods = null;

    [SerializeField]
    private List<DEV_Boss1Mods> boss1Mods = new List<DEV_Boss1Mods>();

    public DEV_Boss1Mods currBoss1Mods = null;

    //TODO: make it so we get all the stuff from the folder instead (for when,
    //we make new ones in there)
    [SerializeField]
    private List<PropertySO> completePSOList = new List<PropertySO>();
    public List<PropertySO> CompletePSOList => completePSOList;

    public void SetMod(DevMenuButtonEnum menuButtonType)
    {
        if (TryFindAndSetMod(ref currTerrMods, terrMods, menuButtonType))
        {
            return;
        }

        if (TryFindAndSetMod(ref currBoss1Mods, boss1Mods, menuButtonType))
        {
            return;
        }
    }

    private bool TryFindAndSetMod<T>(ref T currMod, List<T> modsList, DevMenuButtonEnum menuButtonType) where T : DEV_BuildMods<T>
    {
        T buildMod = GetWrapperFromFunc(modsList, dbm => dbm.MenuButtonType, menuButtonType, LogEnum.NONE, null);
        if (buildMod != null)
        {
            currMod = buildMod;
            return true;
        }

        return false;

    }

    //TODO: mute just music, infinite energy bar, no delay for changing moves, no timer delays, etc.
}


public class DEV_TerrMods : DEV_BuildMods<DEV_TerrMods>
{
    [SerializeField]
    private float speedStartDelta = 0f;
    public float SpeedStartDelta => speedStartDelta;

    [SerializeField]
    private float speedAccelDelta = 0f;
    public float SpeedAccelDelta => speedAccelDelta;

    [SerializeField]
    private float zonePhaseTilDistMultiplyer = 1f;
    public float ZonePhaseTilDistMultiplyer => zonePhaseTilDistMultiplyer;
}


public class DEV_Boss1Mods : DEV_BuildMods<DEV_Boss1Mods>
{
    [SerializeField]
    private RageValue<float> animSwayMultiplyer;
    public RageValue<float> AnimSwayMultiplyer => animSwayMultiplyer;

    [SerializeField]
    private int healthDelta;
    public int HealthDelta => healthDelta;

    [SerializeField]
    private RageValue<float> firePhaseDelayDelta;
    public RageValue<float> FirePhaseDelayDelta => firePhaseDelayDelta;

    [SerializeField]
    private RageValue<float> shootDelayDelta;
    public RageValue<float> ShootDelayDelta => shootDelayDelta;

    [SerializeField]
    private bool hardRageSequence = false;
    public bool HardRageSequence => hardRageSequence;
}

[Serializable]
public class DEV_BuildMods<T>
{
    [SerializeField]
    private DevMenuButtonEnum menuButton = default;
    public DevMenuButtonEnum MenuButtonType => menuButton;
}
