﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]
public class TerrObject : RunnerGameObject
{
    public enum OBJ_TYPE { STATIC, STATIC_HAZ, ENEMY};

    //public enum ACTION { JUMP, ROLL, SPRINT}

    public OBJ_TYPE objType = OBJ_TYPE.STATIC;

    public PLAYER_STATE actionNeeded;

    public int hitBoxUnitWidth = 1;

    public bool canFlip = true;

    public float appearanceLikelihood = 0.2f;

    public float elevationOffsetPerc = 0f;

    public float minHeightUnitsForNextHaz = 0f;


    public void RandomizeSpriteType()
    {
        int index = Random.Range(0, animClips.Length);
        animOC[animIndex] = animClips[index];
    }
}