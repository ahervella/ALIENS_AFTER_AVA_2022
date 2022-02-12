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

    [SerializeField]
    private int floorCount;

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
            taww.Addon.CacheSpawnViolations(floorCount);
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

    public TerrAddonFloorWrapper GetNewAddonFloorWrapper(int colIndex, int rowIndex, Data2D<TerrAddonFloorWrapper> currAddons, SO_TerrSettings settings)
    {
        TerrAddonFloorWrapper newAddonFW = GenerateRandomNewAddonFloorWrapper();

        //have to account for wrapping effect, such that there may be violations in wrapped space
        int wrappedColIndex = colIndex > currAddons.Cols / 2 ? colIndex - currAddons.Cols : colIndex + currAddons.Cols;

        Vector2Int dist2Center;
        Vector2Int wrappedDist2Center;

        for (int x = 0; x < currAddons.Cols; x++)
        {
            for  (int y = 0; y < currAddons.Rows; y++)
            {
                dist2Center = new Vector2Int(colIndex - x, rowIndex - y);
                wrappedDist2Center = new Vector2Int(wrappedColIndex - x, rowIndex);

                if (!FreeOfViolations(currAddons.GetElement(x, y), newAddonFW, dist2Center, wrappedDist2Center))
                {
                    return null;
                }
            }
        }

        return newAddonFW;
    }

    private TerrAddonFloorWrapper GenerateRandomNewAddonFloorWrapper()
    {
        TerrAddon randAddonPrefab = GenerateRandomNewAddon();

        if (randAddonPrefab == null)
        {
            return null;
        }

        int randFloorIndex = Mathf.FloorToInt(Random.value * floorCount);
        return new TerrAddonFloorWrapper(randAddonPrefab, randFloorIndex);
    }

    private TerrAddon GenerateRandomNewAddon()
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

    private bool FreeOfViolations(TerrAddonFloorWrapper source, TerrAddonFloorWrapper other,
        Vector2Int relativePos2Source, Vector2Int wrappedRelativePos2Source)
    {
        if (source == null || other == null) { return true; }

        //TODO: just make the cached stuff a getter to bring all the functionality into TerrAddonFloorWrapper
        return !source.AddonPrefab.IsViolation(source.FloorIndex, other.AddonPrefab, other.FloorIndex, relativePos2Source)
            && !other.AddonPrefab.IsViolation(other.FloorIndex, source.AddonPrefab, source.FloorIndex, -relativePos2Source)
            && !source.AddonPrefab.IsViolation(source.FloorIndex, other.AddonPrefab, other.FloorIndex, wrappedRelativePos2Source)
            && !other.AddonPrefab.IsViolation(other.FloorIndex, source.AddonPrefab, source.FloorIndex, -wrappedRelativePos2Source);
    }
}
