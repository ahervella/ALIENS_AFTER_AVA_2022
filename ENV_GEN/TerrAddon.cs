using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    //x, y, list of prohibited TerrAddons for that cell
    private Dictionary<int,Dictionary<int, List<TerrAddonEnum>>> cachedSpawnViolations
        = new Dictionary<int, Dictionary<int, List<TerrAddonEnum>>>();

    [SerializeField]
    private TerrAddonEnum terrAddonType = default;
    public TerrAddonEnum TerrAddonEnum => terrAddonType;

    public TerrAddon InstantiateAddon(Transform parent)
    {
        TerrAddon taInstance = Instantiate(this, parent);
        taInstance.cachedSpawnViolations = cachedSpawnViolations;
        return taInstance;
    }

    public void CacheSpawnViolations()
    {
        cachedSpawnViolations = new Dictionary<int, Dictionary<int, List<TerrAddonEnum>>>();
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
                    cachedSpawnViolations.Add(xCoor, new Dictionary<int, List<TerrAddonEnum>>());
                }

                Dictionary<int, List<TerrAddonEnum>> col = cachedSpawnViolations[xCoor];


                for (int y = 0; y < dims.y; y++)
                {
                    if (!sr.RuleGrid.GetCell(x, y))
                    {
                        continue;
                    }

                    yCoor = y - sr.CenterIndexPosInRuleGrid.y;

                    if (!col.ContainsKey(yCoor))
                    {
                        col.Add(yCoor, new List<TerrAddonEnum>());
                    }

                    List<TerrAddonEnum> cellAddons = col[yCoor];


                    AddTerrAddonEnum2List(cellAddons, sr.ProhibitedAddon);
                }
            }
        }
    }


    private void AddTerrAddonEnum2List (List<TerrAddonEnum> list, TerrAddonEnum addon)
    {
        if (addon != TerrAddonEnum.ALL_ALIENS
            && addon != TerrAddonEnum.ALL_STATIC_HAZARDS
            && addon != TerrAddonEnum.ALL_RAMPS)
        {
            list.Add(addon);
            return;
        }

        foreach (TerrAddonEnum tae in Enum.GetValues(typeof(TerrAddonEnum)))
        {
            if (tae.GetHashCode() > addon.GetHashCode() && tae.GetHashCode() < addon.GetHashCode() + 100)
            {
                list.Add(tae);
            }
        }
    }


    public bool IsViolation(TerrAddon other, Vector2Int posFromCenter)
    {
        for(int x = 0; x < other.dimensions.x; x++)
        {
            for(int y = 0; y < other.dimensions.y; y++)
            {
                //negative y because we can assume that the y index will always be at the front (0),
                //and for us, looking down on the grid of objects, with the player at the bottom,
                //-y is up
                Vector2Int posRelative2Center = new Vector2Int(x - other.centerXIndex, -y) + posFromCenter;

                Dictionary<int, List<TerrAddonEnum>> col;
                if (!cachedSpawnViolations.TryGetValue(posRelative2Center.x, out col))
                {
                    continue;
                }

                List<TerrAddonEnum> cell;
                if (!col.TryGetValue(posRelative2Center.y, out cell))
                {
                    continue;
                }

                if (cell.Contains(other.TerrAddonEnum))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
