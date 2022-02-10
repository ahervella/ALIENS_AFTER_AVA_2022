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

    private Data2D<List<TerrAddonEnum>> cachedSpawnViolations;

    //x, y, floor, list of violations at that cell and floor
    private Dictionary<int, Dictionary<int, Dictionary<int, List<TerrAddonEnum>>>> cachedSpawnViolationsV2
        = new Dictionary<int, Dictionary<int, Dictionary<int, List<TerrAddonEnum>>>>();

    [SerializeField]
    private TerrAddonEnum terrAddonType = default;
    public TerrAddonEnum TerrAddonEnum => terrAddonType;

    public void CacheSpawnViolations(int maxFloorCount)
    {
        cachedSpawnViolations = new Data2D<List<TerrAddonEnum>>(dimensions.x, dimensions.y, ta => new List<TerrAddonEnum>(), null, null);
        foreach (SpawnRule sr in rules)
        {
            Vector2Int dims = sr.RuleGrid.GridSize;
            int xCoor;
            int yCoor;
            //grids are top down, left to right
            for (int x = 0; x < dims.x; x++)
            {
                xCoor = x - sr.CenterIndexPosInRuleGrid.x;

                if (!cachedSpawnViolationsV2.ContainsKey(xCoor))
                {
                    cachedSpawnViolationsV2.Add(xCoor, new Dictionary<int, Dictionary <int, List<TerrAddonEnum>>>());
                }

                Dictionary<int, Dictionary<int, List<TerrAddonEnum>>> col = cachedSpawnViolationsV2[xCoor];


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


    public bool IsViolation(TerrAddon other, int otherFloorIndex, Vector2Int PosFromCenter)
    {
        for(int x = 0; x < other.dimensions.x; x++)
        {
            for(int y = 0; y < other.dimensions.y; y++)
            {
                Vector2Int posRelative2Center = new Vector2Int(x, y) + PosFromCenter;

                Dictionary<int, Dictionary<int, List<TerrAddonEnum>>> col;
                if (!cachedSpawnViolationsV2.TryGetValue(posRelative2Center.x, out col))
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
    /*
    public void CacheSpawnViolations()
    {
        //max y will always be 0 because rules only apply backwards
        int finalMinX = 0;
        int finalMaxX = 0;
        int finalMinY = 0;

        Vector2Int currMaxRuleGridDims;
        Dictionary<int, Dictionary<int, Dictionary<RuleFloor, List<TerrAddonEnum>>>> ruleGrid = new Dictionary<int, Dictionary<int, Dictionary<RuleFloor, List<TerrAddonEnum>>>>();

        foreach (SpawnRule rule in rules)
        {
            //TODO: change to something more flexi
            int floorCount = 2;

            int minX = 0;
            int maxX = 0;
            int minY = 0;
            int maxY = 0;

            int minMaskX = 0;
            int maxMaskX = 0;
            int minMaskY = 0;
            int maxMaskY = 0;

            switch (rule.Condition)
            {
                case RuleCondition.ADJACENT_HORIZ:
                    minX = rule.LowerLim;
                    maxX = rule.UpperLim;
                    minY = -dimensions.y;

                    for (int x = minX; x < maxX; x++)
                    {
                        if (ruleGrid.ContainsKey(x))
                        {
                            ruleGrid.Add(x, new Dictionary<int, Dictionary<RuleFloor, List<TerrAddonEnum>>>());
                        }

                        Dictionary<int, Dictionary<RuleFloor, List<TerrAddonEnum>>> currCol = ruleGrid[x];

                        for (int y = minY; y < maxY; y++)
                        {
                            if (currCol.ContainsKey(y))
                            {
                                Dictionary<RuleFloor, List<TerrAddonEnum>> coor = new Dictionary<RuleFloor, List<TerrAddonEnum>>
                                {
                                    { RuleFloor.FIRST, new List<TerrAddonEnum>() },
                                    { RuleFloor.SECOND, new List<TerrAddonEnum>() }
                                };

                                currCol.Add(y, coor);
                            }

                            Dictionary <RuleFloor, List<TerrAddonEnum>> currCoord = currCol[y];

                            if (rule.AffectedFloors == RuleFloor.ALL)
                            {
                                currCoord[RuleFloor.FIRST].Add(rule.AllegedType());
                            }
                            else
                            {
                                currCoord[rule.AffectedFloors].Add(rule.AllegedType());
                            }
                        }
                    }
                    break;

                case RuleCondition.ADJACENT_VERT:
                    minX = centerXIndex;
                    maxX = dimensions.x - centerXIndex;
                    minY = rule.LowerLim;
                    maxY = rule.UpperLim;
                    break;

                case RuleCondition.WITHIN_PERIMETER:
                    minX = -rule.UpperLim;
                    maxX = rule.UpperLim;
                    minY = -rule.UpperLim;
                    maxY = rule.UpperLim;

                    minMaskX = -rule.LowerLim;
                    maxMaskX = rule.LowerLim;
                    minMaskY = -rule.UpperLim;
                    maxMaskY = rule.UpperLim;
                    break;
            }

            if (rule.Reference == RuleReference.EDGES)
            {
                minX -= centerXIndex;
                maxX += dimensions.x - centerXIndex;
                minY -= dimensions.y;

                minMaskX -= centerXIndex;
                maxMaskX += dimensions.x - centerXIndex;
                minMaskY -= dimensions.y;
            }


        }
    }*/
}
