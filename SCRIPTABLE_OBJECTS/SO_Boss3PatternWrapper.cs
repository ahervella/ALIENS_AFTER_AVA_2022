using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Array2DEditor;

[CreateAssetMenu(fileName = "SO_Boss3PatternWrapper", menuName = "ScriptableObjects/StaticData/SO_Boss3PatternWrapper")]
public class SO_Boss3PatternWrapper : ScriptableObject
{
    [SerializeField]
    private float nextStepDelay = default;
    public float NextStepDelay => nextStepDelay;

    [SerializeField]
    private float patternDuration = default;
    public float PatterDuration => patternDuration;

    [SerializeField]
    private Array2DBool patternSteps = null;
    public Array2DBool PatternSteps => patternSteps;
}
