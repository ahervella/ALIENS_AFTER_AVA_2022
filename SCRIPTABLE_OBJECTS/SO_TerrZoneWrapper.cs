﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using static HelperUtil;

[CreateAssetMenu(fileName = "SO_TerrZoneWrapper", menuName = "ScriptableObjects/StaticData/SO_TerrZoneWrapper")]
public class SO_TerrZoneWrapper : ScriptableObject
{
    [SerializeField]
    private SO_DeveloperToolsSettings devTools = null;

    [SerializeField]
    private int zone;
    public int Zone => zone;

    //make a tutorial wrapper for the mode and the one shot to be shared here
    //and on the tutorial manager
    [SerializeField]
    private TutorialModeEnum tutorialOnFinish = TutorialModeEnum.NONE;
    public TutorialModeEnum TutorialOnFinish => tutorialOnFinish;

    [SerializeField]
    private BoolPropertySO tutorialOneShotPSO = null;
    public BoolPropertySO TutorialOneShotPSO => tutorialOneShotPSO;

    //[SerializeField]
    //private int floorCount;

    [SerializeField]
    private float startSpeed = default;
    [SerializeField]
    private FloatPropertySO dev_startSpeedMultiplyer = null;

    public float StartSpeed => startSpeed * dev_startSpeedMultiplyer?.Value ?? startSpeed;


    //[SerializeField]
    //private float speedIncPerMin = default;

    //[SerializeField]
    //private float addonSpawnLikelihood = default;
    [SerializeField]
    private FloatPropertySO dev_addonSpawnLikelihoodDelta = null;
    private float AddonSpawnLikelihood =>
        cachedAddonSpawnLikelihood + dev_addonSpawnLikelihoodDelta?.Value ?? cachedAddonSpawnLikelihood;

    //[SerializeField]
    //private float addonSpawnIncPerMin = default;

    [SerializeField]
    private float foleySpawnLikelihood = default;

    [SerializeField]
    private List<ZonePhaseWrapper> terrAddonPhaseWrappers = new List<ZonePhaseWrapper>();

    //Pretty sure we're only gonna need one set foley configs for each zone regardless of phase
    [SerializeField]
    private TerrAddonWeightWrapper[] terrFoleyAddons = default;

    [SerializeField]
    private PSO_CurrentZonePhase currZonePhase = null;

    [Serializable]
    private class TerrAddonWeightWrapper
    {
        [SerializeField]
        private TerrAddon addon = null;
        public TerrAddon Addon => addon;

        [SerializeField]
        private float weight = 0;
        public float Weight => weight;
    }

    [Serializable]
    private class ZonePhaseWrapper
    {
        [SerializeField]
        private bool infiniteTileDist = false;

        [SerializeField]
        private float tileDistanceOfPhase = 0;
        [SerializeField]
        private FloatPropertySO dev_tileDistMultiplyer = null;

        public float? TileDistanceOfPhase => infiniteTileDist ?
            null : (float?)(tileDistanceOfPhase * dev_tileDistMultiplyer?.Value ?? tileDistanceOfPhase);

        [SerializeField]
        private ZonePhaseEnum phase = ZonePhaseEnum.NONE;
        public ZonePhaseEnum Phase => phase;

        [SerializeField]
        private ZonePhaseEnum nextPhase = ZonePhaseEnum.NONE;
        public ZonePhaseEnum NextPhase => nextPhase;

        [SerializeField]
        private SteamAchievementsEnum achievementOnFinish = SteamAchievementsEnum.NONE;
        public SteamAchievementsEnum AchievementOnFinish => achievementOnFinish;

        [SerializeField]
        private float addonSpawnLikelihood = 0f;
        public float AddonSpawnLikelihood => addonSpawnLikelihood;

        [SerializeField]
        private TerrAddonWeightWrapper[] terrAddonWeightWrappers = null;
        public TerrAddonWeightWrapper[] TerrAddonWeightWrappers => terrAddonWeightWrappers;
    }

    [SerializeField]
    private AAlienBossBase bossPrefab = null;
    public AAlienBossBase BossPrefab => bossPrefab;

    [SerializeField]
    private Color envColor = default;
    public Color EnvColor => envColor;

    [NonSerialized]
    private TerrAddonWeightWrapper[] cachedTerrAddons;

    [NonSerialized]
    private float[] TerrAddonCachedPercents;

    [NonSerialized]
    private float[] TerrFoleyAddonCachedPercents;

    [NonSerialized]
    private float cachedAddonSpawnLikelihood;

    private ZonePhaseWrapper GetZonePhaseWrapper(ZonePhaseEnum phase)
    {
        return GetWrapperFromFunc(
            terrAddonPhaseWrappers, pw => pw.Phase, phase, LogEnum.ERROR, null);
    }

    public ZonePhaseEnum GetNextZonePhase(ZonePhaseEnum currPhase)
    {
        return GetZonePhaseWrapper(currPhase).NextPhase;
    }

    public SteamAchievementsEnum GetSteamAchForZonePhase(ZonePhaseEnum phase)
    {
        return GetZonePhaseWrapper(phase).AchievementOnFinish;
    }

    public float? TryGetZonePhaseTileDist(ZonePhaseEnum phase )
    {
        //TODO: add validation here for being infinite if the
        //phase is a transition and there is a tutorial for this wrapper
        //AND the one shot has not been activated
        return GetZonePhaseWrapper(phase).TileDistanceOfPhase;
    }

    public void InitAndCacheTerrAddonData(ZonePhaseEnum phaseOverride = ZonePhaseEnum.NONE)
    {
        if (phaseOverride == ZonePhaseEnum.NONE)
        {
            phaseOverride = currZonePhase.Value;
        }

        ZonePhaseWrapper wrapper = GetZonePhaseWrapper(phaseOverride);

        cachedAddonSpawnLikelihood = wrapper.AddonSpawnLikelihood;
        cachedTerrAddons = wrapper.TerrAddonWeightWrappers;

        CacheWeightPercents();
        CacheTerrSpawnViolations();
    }

    private void CacheWeightPercents()
    {
        TerrAddonCachedPercents = GetWeightPercentsArray(cachedTerrAddons);
        TerrFoleyAddonCachedPercents = GetWeightPercentsArray(terrFoleyAddons);
    }

    private void CacheTerrSpawnViolations()
    {
        foreach (TerrAddonWeightWrapper taww in cachedTerrAddons)
        {
            taww.Addon.CacheSpawnViolations();
        }
    }

    private float[] GetWeightPercentsArray(TerrAddonWeightWrapper[] source)
    {
        float totalWeight = 0;
        foreach (TerrAddonWeightWrapper taww in source)
        {
            totalWeight += taww.Weight;
        }

        float[] cachedPercents = new float[source.Length];
        float totalPerc = 0;

        for (int i = 0; i < source.Length; i++)
        {
            totalPerc += source[i].Weight / totalWeight;
            cachedPercents[i] = totalPerc;
        }

        return cachedPercents;
    }

    

    public TerrAddon GenerateRandomNewAddon(bool getOnlyFoley)
    {
        //first roll dice for spawning any addon
        if (!getOnlyFoley && Random.value < AddonSpawnLikelihood)
        {
            return GetRandomTerrFromWeights(TerrAddonCachedPercents, cachedTerrAddons);
        }

        if (Random.value < foleySpawnLikelihood)
        {
            return GetRandomTerrFromWeights(TerrFoleyAddonCachedPercents, terrFoleyAddons);
        }
        return null;
    }

    private TerrAddon GetRandomTerrFromWeights(float[] cachedPercents, TerrAddonWeightWrapper[] source)
    {
        if (cachedPercents.Length == 0)
        {
            return null;
        }

        float rand = Random.value;
        for (int i = 0; i < cachedPercents.Length; i++)
        {
            if (rand < cachedPercents[i])
            {
                return source[i].Addon;
            }
        }

        Debug.LogError("Something went wrong with cached percents :(");
        return null;
    }
}

public enum ZonePhaseEnum {
    ZONE_INTRO_TRANS = 7,
    END_OF_ZONE = 8,
    NO_BOSS_SUB1 = 0,
    NO_BOSS_SUB2 = 5,
    NO_BOSS_SUB3 = 6,
    BOSS_SPAWN = 3,
    BOSS = 1,
    BOSS_RAGE = 2,
    NONE = 4 }
