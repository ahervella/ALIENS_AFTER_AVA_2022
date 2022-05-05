using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vector2PropertySO", menuName = "ScriptableObjects/Property/Vector2PropertySO")]
public class Vector2PropertySO : PropertySO<Vector2>
{
    public override void ModifyValue(Vector2 mod)
    {
        SetValue(mod);
    }
}
