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
    private TerrAddonEnum allegedType = default;
    public TerrAddonEnum AllegedType() => allegedType;

    [SerializeField]
    private RuleCondition condition = default;
    public RuleCondition Condition => condition;

    [SerializeField]
    private RuleReference reference = default;
    public RuleReference Reference => reference;

    [SerializeField]
    private RuleFloor affectedFloors = default;
    public RuleFloor AffectedFloors => affectedFloors;

    [SerializeField]
    private int lowerLim = default;
    public int LowerLim => lowerLim;

    [SerializeField]
    private int upperLim = default;
    public int UpperLim => upperLim;
}
