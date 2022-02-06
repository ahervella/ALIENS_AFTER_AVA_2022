using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PSO_TargetCameraAngle", menuName = "ScriptableObjects/Property/PSO_TargetCamera")]
public class PSO_TargetCameraAngle : PropertySO<SO_CameraAngle>
{
    public override void ModifyValue(SO_CameraAngle mod)
    {
        if (Value.CameraAngle.FieldOfView == mod.CameraAngle.FieldOfView
            && Value.CameraAngle.PosOffset == mod.CameraAngle.PosOffset
            && Value.CameraAngle.Rot == mod.CameraAngle.Rot)
        {
            return;
        }

        SetValue(mod);
    }
}
