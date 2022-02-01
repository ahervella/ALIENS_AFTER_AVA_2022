using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerrAddon : MonoBehaviour, ITerrNode
{
    [SerializeField]
    private Vector2Int dimensions;
    public Vector2Int Dimensions() => dimensions;

    [SerializeField]
    private int centerXIndex;
    public int CenterXCoor() => centerXIndex;

    [SerializeField]
    private List<SpawnRule> rules;
    public List<SpawnRule> Rules => rules;

    [SerializeField]
    private TerrAddonEnum terrAddonType;
    public TerrAddonEnum TerrAddonEnum => terrAddonType;

}
