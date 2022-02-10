using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerrAddon : MonoBehaviour, ITerrNode
{
    [SerializeField]
    private Vector2Int dimensions = default;
    public Vector2Int Dimensions() => dimensions;

    [SerializeField]
    private int centerXIndex = default;
    public int CenterXCoor() => centerXIndex;

    [SerializeField]
    private List<SpawnRule> rules = new List<SpawnRule>();
    public List<SpawnRule> Rules => rules;

    //x, y, floor, list of violations at that cell and floor
    private Dictionary<int, Dictionary<int, Dictionary<int, List<TerrAddonEnum>>>> cachedSpawnViolations
        = new Dictionary<int, Dictionary<int, Dictionary<int, List<TerrAddonEnum>>>>();

    [SerializeField]
    private TerrAddonEnum terrAddonType = default;
    public TerrAddonEnum TerrAddonEnum => terrAddonType;

    public void CacheSpawnViolations(int maxFloorCount)
    {
        cachedSpawnViolations = new Dictionary<int, Dictionary<int, Dictionary<int, List<TerrAddonEnum>>>>();
        foreach (SpawnRule sr in rules)
        {
            Vector2Int dims = sr.RuleGrid.GridSize;
            int xCoor;
            int yCoor;
            //grids are top down, left to right
            for (int x = 0; x < dims.x; x++)
            {
                xCoor = x - sr.CenterIndexPosInRuleGrid.x;

                if (!cachedSpawnViolations.ContainsKey(xCoor))
                {
                    cachedSpawnViolations.Add(xCoor, new Dictionary<int, Dictionary <int, List<TerrAddonEnum>>>());
                }

                Dictionary<int, Dictionary<int, List<TerrAddonEnum>>> col = cachedSpawnViolations[xCoor];


                for (int y = 0; y < dims.y; y++)
                {
                    if (!sr.RuleGrid.GetCell(x, y))
                    {
                        continue;
                    }

                    yCoor = y - sr.CenterIndexPosInRuleGrid.y;

                    if (!col.ContainsKey(yCoor))
                    {
                        col.Add(yCoor, new Dictionary<int, List<TerrAddonEnum>>());
                    }

                    Dictionary<int, List<TerrAddonEnum>> cellFloors = col[yCoor];


                    if (sr.ApplyToAllFloors)
                    {
                        for (int i = 0; i < maxFloorCount; i++)
                        {
                            if (!cellFloors.ContainsKey(i))
                            {
                                cellFloors.Add(i, new List<TerrAddonEnum>());
                            }

                            cellFloors[i].Add(sr.ProhibtedAddon);
                        }
                        continue;
                    }

                    if (!cellFloors.ContainsKey(sr.FloorAppliedTo))
                    {
                        cellFloors.Add(sr.FloorAppliedTo, new List<TerrAddonEnum>());
                    }

                    cellFloors[sr.FloorAppliedTo].Add(sr.ProhibtedAddon);
                }
            }
        }
    }


    public bool IsViolation(TerrAddon other, int otherFloorIndex, Vector2Int posFromCenter)
    {
        for(int x = 0; x < other.dimensions.x; x++)
        {
            for(int y = 0; y < other.dimensions.y; y++)
            {
                Vector2Int posRelative2Center = new Vector2Int(x, y) + posFromCenter;

                Dictionary<int, Dictionary<int, List<TerrAddonEnum>>> col;
                if (!cachedSpawnViolations.TryGetValue(posRelative2Center.x, out col))
                {
                    continue;
                }

                Dictionary<int, List<TerrAddonEnum>> cell;
                if (!col.TryGetValue(posRelative2Center.y, out cell))
                {
                    continue;
                }

                if (cell[otherFloorIndex].Contains(other.TerrAddonEnum))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
