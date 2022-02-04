using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SO_CAW-", menuName = "ScriptableObjects/StaticData/SO_CameraAngleWrapper")]
[Serializable]
public class SO_CameraAngleWrapper : ScriptableObject
{
    [SerializeField]
    private CameraAngle cameraAngle = null;
    public CameraAngle CameraAngle => cameraAngle;

    [SerializeField]
    private PlayerActionEnum action = PlayerActionEnum.NONE;
    public PlayerActionEnum Action => action;
}
