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

    public TerrAddonFloorWrapper GetNewAddonFloorWrapper(int colIndex, int rowIndex, Data2D<TerrAddonFloorWrapper> currAddons)
    {
        if (cachedZoneWrapper == null)
        {
            OnZoneChange(-1, currZone.Value);
        }

        return cachedZoneWrapper.GetNewAddonFloorWrapper(colIndex, rowIndex, currAddons, settings);
    }
}