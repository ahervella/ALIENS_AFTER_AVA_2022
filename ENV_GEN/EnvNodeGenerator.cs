using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvNodeGenerator : MonoBehaviour
{
    [SerializeField]
    private SO_TerrSettings settings = null;

    [SerializeField]
    private IntPropertySO currZone = null;

    [SerializeField]
    private List<SO_TerrZoneWrapper> zoneWrappers = null;

    private SO_TerrZoneWrapper cachedZoneWrapper;

    private void Awake()
    {
        currZone.RegisterForPropertyChanged(OnZoneChange);
    }

    private void OnZoneChange(int prevZone, int newZone)
    {
        foreach (SO_TerrZoneWrapper zw in zoneWrappers)
        {
            if (zw.Zone == newZone)
            {
                cachedZoneWrapper = zw;
                cachedZoneWrapper.CacheWeightPercents();
                cachedZoneWrapper.CacheTerrSpawnViolations();
                return;
            }
        }

        Debug.LogError($"Could not find zone wrapper for zone {newZone} :(");
    }

    public TerrAddon GetNewAddon(int colIndex, int rowIndex, Data2D<TerrAddon> currAddons)
    {
        if (cachedZoneWrapper == null)
        {
            OnZoneChange(-1, currZone.Value);
        }

        return TryGetNewViolationFreeAddon(colIndex, rowIndex, currAddons);
    }

    private TerrAddon TryGetNewViolationFreeAddon(int colIndex, int rowIndex, Data2D<TerrAddon> currAddons)
    {
        TerrAddon newAddon = cachedZoneWrapper.GenerateRandomNewAddon();
        if (newAddon == null) { return null; }

        //have to account for wrapping effect, such that there may be violations in wrapped space
        int wrappedColIndex = colIndex > currAddons.Cols / 2 ? colIndex - currAddons.Cols : colIndex + currAddons.Cols;

        Vector2Int dist2Center;
        Vector2Int wrappedDist2Center;

        for (int x = 0; x < currAddons.Cols; x++)
        {
            for (int y = 0; y < currAddons.Rows; y++)
            {
                TerrAddon sourceAddon = currAddons.GetElement(x, y);
                if (sourceAddon == null) { continue; }

                dist2Center = new Vector2Int(colIndex - x, rowIndex - y);
                wrappedDist2Center = new Vector2Int(wrappedColIndex - x, rowIndex - y);

                if (!FreeOfViolations(sourceAddon, newAddon, dist2Center, wrappedDist2Center))
                {
                    return null;
                }
            }
        }

        return newAddon;
    }



    private bool FreeOfViolations(TerrAddon source, TerrAddon other,
        Vector2Int relativePos2Source, Vector2Int wrappedRelativePos2Source)
    {
        if (source == null || other == null) { return true; }

        //TODO: just make the cached stuff a getter to bring all the functionality into TerrAddon
        return !source.IsViolation(other, relativePos2Source)
            && !other.IsViolation(source, -relativePos2Source)
            && !source.IsViolation(other, wrappedRelativePos2Source)
            && !other.IsViolation(source, -wrappedRelativePos2Source);
    }
}