using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*Violation if [alleged]
 * is [condition]
 * with respect to [reference]
 * on [affectedFloors]
 * by a magnitude from [lowerLim] to [upperLim]*/

public class SpawnRule : MonoBehaviour
{
    [SerializeField]
    private TerrAddonEnum allegedType;
    public TerrAddonEnum AllegedType() => allegedType;

    [SerializeField]
    private RuleCondition condition;
    public RuleCondition Condition => condition;

    [SerializeField]
    private RuleReference reference;
    public RuleReference Reference => reference;

    [SerializeField]
    private RuleFloor affectedFloors;
    public RuleFloor AffectedFloors => affectedFloors;

    [SerializeField]
    private int lowerLim;
    public int LowerLim => lowerLim;

    [SerializeField]
    private int upperLim;
    public int UpperLim => upperLim;
}

public enum RuleCondition
{
    WITHIN_PERIMETER, ADJACENT, INFRONT_OF, BEHIND
}

public enum RuleReference
{
    EDGES, CENTER
}

public enum RuleFloor
{
    FIRST, SECOND, ALL
}
