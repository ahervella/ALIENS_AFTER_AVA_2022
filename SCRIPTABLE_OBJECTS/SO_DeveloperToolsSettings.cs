using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static HelperUtil;
using UnityEditor;
using Object = UnityEngine.Object;

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
    private bool noReqsForArmaments = false;
    public bool NoReqsForArmaments => noReqsForArmaments;

    [SerializeField]
    private bool allArmamentsAvailable = false;
    public bool AllArmamentsAvailable => allArmamentsAvailable;

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

    public void SetSpawnBossOnStart(bool spawnBossOnStart)
    {
        this.spawnBossOnStart = spawnBossOnStart;
    }

    [SerializeField]
    private bool skipTutorials = false;
    public bool SkipTutorials => skipTutorials;

    //TODO: make into a dev menu button and dynamic PSO listener
    [SerializeField]
    private bool muteMusic = false;
    public bool MuteMusic => muteMusic;

    [SerializeField]
    private bool demoMode = false;
    public bool DemoMode => demoMode;

    [SerializeField]
    private Loadout firstLoadout = null;
    public Loadout FirstLoadout => firstLoadout;

    [SerializeField]
    private TerrModPSOs terrModPSOs = new TerrModPSOs();

    [SerializeField]
    private List<TerrModPreset> terrMods = new List<TerrModPreset>();

    [NonSerialized]
    private TerrModPreset currTerrMods = null;
    public TerrModPreset CurrTerrMods => currTerrMods;

    [SerializeField]
    private Boss1ModPSOs boss1PSOs = new Boss1ModPSOs();

    [SerializeField]
    private List<Boss1ModPreset> boss1Mods = new List<Boss1ModPreset>();

    [NonSerialized]
    public Boss1ModPreset CurrBoss1Mods = null;

    //TODO: make it so we get all the stuff from the folder instead (for when,
    //we make new ones in there), WITH scrolling, which is being a biatch
    [SerializeField]
    private List<PropertySO> exposedPSOs = new List<PropertySO>();
    public List<PropertySO> ExposedPSOs => exposedPSOs;


    /*
    [NonSerialized]
    private List<PropertySO> completePSOList = null;
    public List<PropertySO> GetCompletePSOList()
    {
        if (completePSOList == null)
        {
            CacheCompletePSOList();
        }

        return completePSOList;
    }


    [SerializeField]
    private string psoDirectory = string.Empty;

    public void CacheCompletePSOList()
    {
        completePSOList = new List<PropertySO>();
        string[] psos = AssetDatabase.FindAssets($"t:{typeof(PropertySO)}", new[] { psoDirectory });
        foreach (string psoGUID in psos)
        {
            string psoPath = AssetDatabase.GUIDToAssetPath(psoGUID);
            PropertySO castedPSO = AssetDatabase.LoadAssetAtPath<PropertySO>(psoPath);
            if (castedPSO == null) { continue; }
            completePSOList.Add(castedPSO);
        }
    }*/


    public string TryGetModName(DevMenuButtonEnum menuButtonType)
    {
        BuildModPreset mod = TryGetMod(menuButtonType);
        return mod?.PresetName;
    }


    public bool TrySetMod(DevMenuButtonEnum menuButtonType)
    {
        BuildModPreset mod = TryGetMod(menuButtonType);
        if (mod != null)
        {
            mod.SetThisMod(this);
            return true;
        }

        return false;
    }

    private BuildModPreset TryGetMod(DevMenuButtonEnum menuButtonType)
    {
        TerrModPreset terrMod = TryGetModFromList(terrMods, menuButtonType);
        if (terrMod != null)
        {
            return terrMod;
        }

        Boss1ModPreset boss1Mod = TryGetModFromList(boss1Mods, menuButtonType);
        if (boss1Mod != null)
        {
            return boss1Mod;
        }

        return null;
    }

    private T TryGetModFromList<T>(List<T> modsList, DevMenuButtonEnum menuButtonType) where T : BuildModPreset
    {
        return GetWrapperFromFunc(modsList, dbm => dbm.MenuButtonType, menuButtonType, LogEnum.NONE, null);
    }


    //TODO: mute just music, infinite energy bar, no delay for changing moves, no timer delays, etc.






    [Serializable]
    private class TerrModPSOs : AModPSOs<TerrModPreset>
    {
        [SerializeField]
        private FloatPropertySO speedStartMultiplyer = null;

        [SerializeField]
        private Vector2PropertySO tileDimDelta = null;

        [SerializeField]
        private FloatPropertySO zonePhaseTileDistMultiplyer = null;

        [SerializeField]
        private FloatPropertySO addonSpawnLikelihoodDelta = null;

        public override void SetMod(TerrModPreset modPreset)
        {
            speedStartMultiplyer.DirectlySetValue(modPreset.SpeedStartMultiplyer);

            tileDimDelta.ModifyValue(modPreset.TileDimDelta);

            zonePhaseTileDistMultiplyer.DirectlySetValue(modPreset.SpeedStartMultiplyer);

            addonSpawnLikelihoodDelta.DirectlySetValue(modPreset.AddonSpawnLikelihoodDelta);
        }
    }

    [Serializable]
    public class TerrModPreset : BuildModPreset
    {
        public override void SetThisMod(SO_DeveloperToolsSettings devTools)
        {
            devTools.terrModPSOs.SetMod(this);
        }

        [SerializeField]
        private float speedStartMultiplyer = 0f;
        public float SpeedStartMultiplyer => speedStartMultiplyer;

        //TODO: still need to implement slow acceleration if want it
        //[SerializeField]
        //private float speedAccelDelta = 0f;
        //public float SpeedAccelDelta => speedAccelDelta;

        [SerializeField]
        private Vector2 tileDimDelta = new Vector2(0, 0);
        public Vector2 TileDimDelta => tileDimDelta;

        [SerializeField]
        private float zonePhaseTileDistMultiplyer = 1f;
        public float ZonePhaseTileDistMultiplyer => zonePhaseTileDistMultiplyer;

        [SerializeField]
        private float addonSpawnLikelihoodDelta = 0f;
        public float AddonSpawnLikelihoodDelta => addonSpawnLikelihoodDelta;
    }

    [Serializable]
    private class Boss1ModPSOs : AModPSOs<Boss1ModPreset>
    {
        //[SerializeField]
        //private RageFloatPSO animSwayMultiplyer = null;

        [SerializeField]
        private IntPropertySO healthDelta = null;

        [SerializeField]
        private IntPropertySO rageHealthThreshold = null;

        [SerializeField]
        private RageFloatPSO firePhaseDelayDelta = null;

        [SerializeField]
        private RageFloatPSO fireShotRandDelayRange = null;

        [SerializeField]
        private RageFloatPSO shootDelayDeltaDelta = null;

        //[SerializeField]
        //private BoolPropertySO easyShootSequence = null;

        public override void SetMod(Boss1ModPreset modPreset)
        {
            healthDelta.DirectlySetValue(modPreset.HealthDelta);

            rageHealthThreshold.DirectlySetValue(modPreset.RageHealthThresholdDelta);

            firePhaseDelayDelta.ModifyValue(modPreset.FirePhaseDelayDelta);

            fireShotRandDelayRange.ModifyValue(modPreset.FireShotRandDelayRange);

            shootDelayDeltaDelta.ModifyValue(modPreset.ShootDelayDelta);

            //easyShootSequence.ModifyValue(modPreset.EasyShootSequence);
        }
    }

    [Serializable]
    public class Boss1ModPreset : BuildModPreset//<Boss1ModPreset>
    {
        public override void SetThisMod(SO_DeveloperToolsSettings devTools)
        {
            devTools.boss1PSOs.SetMod(this);
        }

        //[SerializeField]
        //private RageValue<float> animSwayMultiplyer;
        //public RageValue<float> AnimSwayMultiplyer => animSwayMultiplyer;

        [SerializeField]
        private int healthDelta = 0;
        public int HealthDelta => healthDelta;

        [SerializeField]
        private int rageHealthThresholdDelta;
        public int RageHealthThresholdDelta => rageHealthThresholdDelta;

        [SerializeField]
        private RageValue<float> firePhaseDelayDelta;
        public RageValue<float> FirePhaseDelayDelta => firePhaseDelayDelta;

        [SerializeField]
        private RageValue<float> fireShotRandDelayRange;
        public RageValue<float> FireShotRandDelayRange => fireShotRandDelayRange;

        [SerializeField]
        private RageValue<float> shootDelayDelta;
        public RageValue<float> ShootDelayDelta => shootDelayDelta;

        //[SerializeField]
        //private bool easyShootSequence = false;
        //public bool EasyShootSequence => easyShootSequence;
    }

    [Serializable]
    public abstract class BuildModPreset//<T>
    {
        [SerializeField]
        private string presetName = "UNAMED_PRESET";
        public string PresetName => presetName;

        [SerializeField]
        private DevMenuButtonEnum menuButton = default;
        public DevMenuButtonEnum MenuButtonType => menuButton;

        public abstract void SetThisMod(SO_DeveloperToolsSettings devTools);
    }

    public abstract class AModPSOs<T> where T : BuildModPreset
    {
        public abstract void SetMod(T modPreset);
    }
}


