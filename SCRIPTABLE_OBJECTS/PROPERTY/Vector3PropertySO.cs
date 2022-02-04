using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Vector3PropertySO", menuName = "ScriptableObjects/Property/Vector3PropertySO")]
public class Vector3PropertySO : PropertySO<Vector3>
{
    public override void ModifyValue(Vector3 mod)
    {
        SetValue(mod);
    }
}
