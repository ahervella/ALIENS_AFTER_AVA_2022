using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DSO_TerrainChange", menuName = "ScriptableObjects/Delegates/DSO_TerrainChange")]
public class DSO_TerrainChange : DelegateSO<TerrainChangeWrapper, int>
{
}

public class TerrainChangeWrapper
{
    public TerrainChangeWrapper(bool DueToZoneOrPhaseChange)
    {
        this.DueToZoneOrPhaseChange = DueToZoneOrPhaseChange;
    }

    public bool DueToZoneOrPhaseChange { get; private set; }
}
