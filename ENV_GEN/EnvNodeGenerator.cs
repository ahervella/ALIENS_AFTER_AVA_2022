using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvNodeGenerator : MonoBehaviour
{
    [SerializeField]
    private SO_TerrSettings settings;

    [SerializeField]
    private List<SO_TerrZoneWrapper> zoneWrappers;

    public TerrAddon GetNewAddon(int colIndex, int rowIndex, Data2D<TerrAddon[]> currAddons)
    {
        return default;
    }


}