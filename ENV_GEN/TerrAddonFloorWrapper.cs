using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrAddonFloorWrapper
{
    public TerrAddon AddonPrefab { get; private set; }
    public TerrAddon AddonInst { get; private set; }
    public int FloorIndex { get; private set; }

    public TerrAddonFloorWrapper(TerrAddon addonPrefab, int floorIndex)
    {
        AddonPrefab = addonPrefab;
        AddonInst = null;
        FloorIndex = floorIndex;
    }

    public TerrAddon InstantiateFromPrefab(Transform parent)
    {
        AddonInst = GameObject.Instantiate(AddonPrefab, parent);
        return AddonInst;
    }

    public void DestroyInstance()
    {
        GameObject.Destroy(AddonInst.gameObject);
        AddonInst = null;
    }
}
