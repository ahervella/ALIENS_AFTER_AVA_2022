using UnityEngine;

[CreateAssetMenu(fileName = "_FloatPropertySO", menuName = "ScriptableObjects/Property/FloatPropertySO", order = 2)]
public class FloatPropertySO : PropertySO<float>
{
    [SerializeField]
    private float maxValue = float.MaxValue;

    [SerializeField]
    private float minValue = float.MinValue;

    [SerializeField]
    private FloatPropertySO maxValueSO = null;

    [SerializeField]
    private FloatPropertySO minValueSO = null;

    public override void ModifyValue(float change)
    {
        float min = minValueSO == null ? minValue : minValueSO.Value;
        float max = maxValueSO == null ? maxValue : maxValueSO.Value;

        if (Value > max || Value < min)
        {
            return;
        }

        if (Value + change > max)
        {
            change = max - Value;
        }
        if (Value - change < min)
        {
            change = min - Value;
        }
        SetValue(Value + change);
    }
}