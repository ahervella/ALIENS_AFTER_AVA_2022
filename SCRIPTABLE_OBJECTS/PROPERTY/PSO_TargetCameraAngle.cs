using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PSO_TargetCameraAngle", menuName = "ScriptableObjects/Property/PSO_TargetCamera")]
public class PSO_TargetCameraAngle : PropertySO<CameraAngle>
{
    public override void ModifyValue(CameraAngle mod)
    {
        SetValue(mod);
    }
}
