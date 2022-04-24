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
    private int zone;
    public int Zone => zone;

    //[SerializeField]
    //private int floorCount;

    [SerializeField]
    private float startSpeed = default;
    public float StartSpeed => startSpeed;

    //[SerializeField]
    //private float speedIncPerMin = default;

    [SerializeField]
    private float addonSpawnLikelihood = default;

    //[SerializeField]
    //private float addonSpawnIncPerMin = default;

    [SerializeField]
    private float foleySpawnLikelihood = default;

    [SerializeField]
    private float tileDistance2Boss = default;
    public float TileDistance2Boss => tileDistance2Boss;

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
        private ZonePhaseEnum phase = ZonePhaseEnum.NO_BOSS;
        public ZonePhaseEnum Phase => phase;

        [SerializeField]
        private TerrAddonWeightWrapper[] terrAddonWeightWrappers = null;
        public TerrAddonWeightWrapper[] TerrAddonWeightWrappers => terrAddonWeightWrappers;
    }

    [SerializeField]
    private AAlienBossBase bossPrefab = null;
    public AAlienBossBase BossPrefab => bossPrefab;

    [NonSerialized]
    private TerrAddonWeightWrapper[] cachedTerrAddons;

    [NonSerialized]
    private float[] TerrAddonCachedPercents;

    [NonSerialized]
    private float[] TerrFoleyAddonCachedPercents;

    public void InitAndCacheTerrAddonData()
    {
        CacheTerrAddonWeightWrapper();
        CacheWeightPercents();
        CacheTerrSpawnViolations();
    }

    private void CacheTerrAddonWeightWrapper()
    {
        cachedTerrAddons = GetWrapperFromFunc(
            terrAddonPhaseWrappers, pw => pw.Phase, currZonePhase.Value, LogEnum.ERROR, null).TerrAddonWeightWrappers;
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
        if (!getOnlyFoley && Random.value < addonSpawnLikelihood)
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

public enum ZonePhaseEnum { NO_BOSS = 0, BOSS_SPAWN = 3, BOSS = 1, BOSS_RAGE = 2, NONE = 4 }
