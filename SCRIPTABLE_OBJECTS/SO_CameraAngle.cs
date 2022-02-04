using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_CA-", menuName = "ScriptableObjects/StaticData/SO_CameraAngle")]
public class SO_CameraAngle : ScriptableObject
{
    public SO_CameraAngle(float fieldOfView, Vector3 posOffset, Vector3 rot, float tweenTime, PlayerActionEnum actionTag)
    {
        this.fieldOfView = fieldOfView;
        this.posOffset = posOffset;
        this.rot = rot;
        this.tweenTime = tweenTime;
        this.actionTag = actionTag;
    }


    [SerializeField]
    private FloatPropertySO baseFieldOfView = null;
    [SerializeField]
    private float fieldOfView = 0;
    public float FieldOfView => baseFieldOfView == null ? fieldOfView : baseFieldOfView.Value + fieldOfView;

    [SerializeField]
    private Vector3PropertySO basePosOffset = null;
    [SerializeField]
    private Vector3 posOffset = default;
    public Vector3 PosOffset => basePosOffset == null ? posOffset : basePosOffset.Value + posOffset;

    [SerializeField]
    private Vector3 rot = default;
    public Vector3 Rot => rot;

    [SerializeField]
    private float tweenTime = 1f;
    public float TweenTime => tweenTime;

    [SerializeField]
    private float tweenDelay = 0f;
    public float TweenDelay => tweenDelay;

    [SerializeField]
    private PlayerActionEnum actionTag = PlayerActionEnum.NONE;
    public PlayerActionEnum ActionTag => actionTag;
}
