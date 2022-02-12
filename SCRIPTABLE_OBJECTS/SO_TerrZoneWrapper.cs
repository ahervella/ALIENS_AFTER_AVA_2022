using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

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

    [SerializeField]
    private TerrAddonWeightWrapper[] TerrAddons = null;

    [SerializeField]
    private TerrAddonWeightWrapper[] TerrFoleyAddons = null;

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

    [NonSerialized]
    private float[] TerrAddonCachedPercents;

    [NonSerialized]
    private float[] TerrFoleyAddonCachedPercents;

    public void CacheWeightPercents()
    {
        TerrAddonCachedPercents = GetWeightPercentsArray(TerrAddons);
        TerrFoleyAddonCachedPercents = GetWeightPercentsArray(TerrFoleyAddons);
    }

    public void CacheTerrSpawnViolations()
    {
        foreach (TerrAddonWeightWrapper taww in TerrAddons)
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

    

    public TerrAddon GenerateRandomNewAddon()
    {
        //first roll dice for spawning any addon
        if (Random.value < addonSpawnLikelihood)
        {
            return GetRandomTerrFromWeights(TerrAddonCachedPercents, TerrAddons);
        }

        if (Random.value < foleySpawnLikelihood)
        {
            return GetRandomTerrFromWeights(TerrFoleyAddonCachedPercents, TerrFoleyAddons);
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
