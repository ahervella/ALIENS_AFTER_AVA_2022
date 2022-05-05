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

    //TODO: edit code to use this throughout, set up virtual in base or
    //other solution and change on all PSOs
    public void DirectlySetValue(int newVal)
    {
        ModifyValue(newVal - Value);
    }

    public override void ModifyValue(float change)
    {
        float min = minValueSO == null ? minValue : minValueSO.Value;
        float max = maxValueSO == null ? maxValue : maxValueSO.Value;

        SetValue(Mathf.Clamp(Value + change, min, max));
    }

    public void DirectlySetValue(float newValue)
    {
        ModifyValue(newValue - Value);
    }
}