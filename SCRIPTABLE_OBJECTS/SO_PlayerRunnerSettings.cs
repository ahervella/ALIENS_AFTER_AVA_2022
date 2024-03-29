﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "SO_PlayerRunnerSettings", menuName = "ScriptableObjects/StaticData/SO_PlayerRunnerSettings")]
public class SO_PlayerRunnerSettings : ScriptableObject
{
    //TODO: evaluate once working runner level if we want to break these up
    [SerializeField]
    private float lifeRecoveryTime;
    public float LifeRecoveryTime => lifeRecoveryTime;

    [SerializeField]
    private float sprintTime = 1;
    public float SprintTime => sprintTime;

    [SerializeField]
    private float laneChangeTime;
    public float LaneChangeTime => laneChangeTime;

    [SerializeField]
    private float laneChangeDelay;
    public float LaneChangeDelay => laneChangeDelay;

    [SerializeField]
    private float startRowsFromEnd = 1;
    public float StartRowsFromEnd => startRowsFromEnd;

    [SerializeField]
    private Vector3 startPosOffset = default;
    public Vector3 StartPosOffset => startPosOffset;

    [SerializeField]
    private float postHurtInvincibilityTime = 1f;
    public float PostHurtInvincibilityTime => postHurtInvincibilityTime;
}
