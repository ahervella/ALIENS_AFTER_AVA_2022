using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CameraAngle
{
    public CameraAngle(float fieldOfView, Vector3 posOffset, Vector3 rotOffset, float tweenTime)
    {
        this.fieldOfView = fieldOfView;
        this.posOffset = posOffset;
        this.rotOffset = rotOffset;
        this.tweenTime = tweenTime;
    }


    [SerializeField]
    private FloatPropertySO baseFieldOfView = null;
    [SerializeField]
    private float fieldOfView = 0;
    public float FieldOfView => baseFieldOfView == null? fieldOfView : baseFieldOfView.Value + fieldOfView;

    [SerializeField]
    private Vector3PropertySO basePosOffset = null;
    [SerializeField]
    private Vector3 posOffset = default;
    public Vector3 PosOffset => basePosOffset == null? posOffset : basePosOffset.Value + posOffset;

    [SerializeField]
    private Vector3 rotOffset = default;
    public Vector3 RofOffset => rotOffset;

    [SerializeField]
    private float tweenTime = 1f;
    public float TweenTime => tweenTime;

    [SerializeField]
    private float tweenDelay = 0f;
    public float TweenDelay => tweenDelay;
}
