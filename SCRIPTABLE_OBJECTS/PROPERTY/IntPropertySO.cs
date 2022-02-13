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
        int min = minValueSO == null ? minValue : minValueSO.Value;
        int max = maxValueSO == null ? maxValue : maxValueSO.Value;

        SetValue(Mathf.Clamp(Value + change, min, max));
    }
}