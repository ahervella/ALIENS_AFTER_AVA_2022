using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Array2DEditor;
using System;

[Serializable]
public class SpawnRule
{
    [SerializeField]
    private TerrAddonEnum prohibtedAddon;
    public TerrAddonEnum ProhibtedAddon => prohibtedAddon;

    [SerializeField]
    private int floorAppliedTo;
    public int FloorAppliedTo => floorAppliedTo;

    [SerializeField]
    private bool applyToAllFloors;
    public bool ApplyToAllFloors => applyToAllFloors;

    [SerializeField]
    private Array2DBool ruleGrid = null;
    public Array2DBool RuleGrid => ruleGrid;

    [SerializeField]
    private Vector2Int centerIndexPosInRuleGrid = default;
    public Vector2Int CenterIndexPosInRuleGrid => centerIndexPosInRuleGrid;
}
