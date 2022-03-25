using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_TerrSettings", menuName = "ScriptableObjects/StaticData/SO_TerrSettings")]
public class SO_TerrSettings : ScriptableObject
{

    [SerializeField]
    private Vector2 tileDims;
    public Vector2 TileDims => tileDims;

    [SerializeField]
    private float floorHeight;
    public float FloorHeight => floorHeight;

    [SerializeField]
    private int floorCount;
    public int FloorCount => floorCount;

    //TODO: do we need this lane count if we have the other dimension still?
    //at least simplify it
    [SerializeField]
    private int laneCount;
    public int LaneCount => laneCount;

    [SerializeField]
    private int tileCols;
    public int TileCols => tileCols;

    [SerializeField]
    private int tileRows;
    public int TileRows => tileRows;

    public int PointCols => TileCols + 1;

    public int PointRows => TileRows + 1;

    [SerializeField]
    private int interCount;
    public int InterCount => interCount;

    public int InterCols => PointCols + TileCols * InterCount;

    public int InterRows => PointRows + TileRows * InterCount;

    [SerializeField]
    private float rewardBoxTileLengthFront = 1;
    public float RewardBoxLengthFront => rewardBoxTileLengthFront * tileDims.y;

    [SerializeField]
    private int startHazardFreeRowsFromPlayer = 1;
    public int StartHazardFreeRowsFromPlayer => startHazardFreeRowsFromPlayer;
}
