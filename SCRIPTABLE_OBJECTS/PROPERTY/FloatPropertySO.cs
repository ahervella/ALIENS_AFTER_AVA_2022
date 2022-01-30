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

        if (Value < max && Value > min)
        {
            SetValue(Mathf.Clamp(Value + change, min, max));
            return;
        }

        if (Value > max && change < 0)
        {
            SetValue(Value + change);
            return;
        }

        if (Value < min && change > 0)
        {
            SetValue(Value + change);
            return;
        }
    }
}