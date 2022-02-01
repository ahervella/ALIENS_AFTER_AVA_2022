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

    [SerializeField]
    private TerrAddonEnum terrAddonType = default;
    public TerrAddonEnum TerrAddonEnum => terrAddonType;

}
