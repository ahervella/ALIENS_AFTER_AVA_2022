using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_TerrSettings : ScriptableObject
{
    public Vector2 TileDims { get; private set; }

    public float FloorHeight { get; private set; }

    public int FloorCount { get; private set; }

    public int LaneCount { get; private set; }

    public int TileRows { get; private set; }

    public int TileCols { get; private set; }

    [HideInInspector]
    public int PointRows => TileRows + 1;

    [HideInInspector]
    public int PointCols => TileCols + 1;

    public int InterCount { get; private set; }

    public int InterRows => PointRows + TileRows * InterCount;

    public int InterCols => PointCols + TileCols * InterCount;
}
