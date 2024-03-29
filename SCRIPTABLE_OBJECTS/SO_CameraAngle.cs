﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SO_CA-", menuName = "ScriptableObjects/StaticData/SO_CameraAngle")]
public class SO_CameraAngle : ScriptableObject
{
    [SerializeField]
    private CameraAngle cameraAngle;
    public CameraAngle CameraAngle => cameraAngle;
}

[Serializable]
public class CameraAngle
{
    public CameraAngle(float fieldOfView, Vector3 posOffset, Vector3 rot, float tweenTime)
    {
        this.fieldOfView = fieldOfView;
        this.posOffset = posOffset;
        this.rot = rot;
        this.tweenTime = tweenTime;
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
}
