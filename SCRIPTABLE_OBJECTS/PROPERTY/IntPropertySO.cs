using System;
using UnityEngine;

[CreateAssetMenu(fileName = "_IntPropertySO", menuName = "ScriptableObjects/Property/IntPropertySO", order = 2)]
public class IntPropertySO : PropertySO<int>
{

    [SerializeField]
    private int maxValue = int.MaxValue;

    [SerializeField]
    private int minValue = int.MinValue;

    [SerializeField]
    private IntPropertySO maxValueSO = null;

    [SerializeField]
    private IntPropertySO minValueSO = null;

    public override void ModifyValue(int change)
    {
        SetValue(Mathf.Clamp(Value + change, MinValue(), MaxValue()));
    }

    public int MaxValue()
    {
        return maxValueSO == null ? maxValue : maxValueSO.Value;
    }

    public int MinValue()
    {
        return minValueSO == null ? minValue : minValueSO.Value;
    }
}