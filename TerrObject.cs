﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]
public class TerrObject : RunnerGameObject
{
    public static event System.Action<TerrObject> terrObjDestroyed = delegate { };

    public enum OBJ_TYPE { STATIC, STATIC_HAZ, ROCK, TEMP_GUN, ENEMY, THROWABLE};

    public const float ATTACHMENT_SPACING = 0.2f;
    //public enum ACTION { JUMP, ROLL, SPRINT}

    public OBJ_TYPE objType = OBJ_TYPE.STATIC;

    public PLAYER_STATE actionNeeded = PLAYER_STATE.NONE;

    public int hitBoxUnitWidth = 1;

    public bool canFlip = true;

    public float appearanceLikelihood = 0.2f;

    public float elevationOffsetPerc = 0f;

    public bool centerXPosWithHitBox = false;

    public int laneOffset = 0;

    public float minHeightUnitsForNextHaz = 0f;

    public bool canHaveAttachments = false;

    public List<TerrObject> AttachmentObjects { get; set; } = new List<TerrObject>();

    public void RandomizeSpriteType()
    {
        int index = Random.Range(0, animClips.Length);
        animOC[animIndex] = animClips[index];
    }

    public Vector3 getOffsetFromHitBox()
    {
        return gameObject.GetComponent<BoxCollider>().center * gameObject.transform.localScale.x;
    }

    private void OnDestroy()
    {
        foreach(TerrObject attachment in AttachmentObjects)
        {
            Destroy(attachment);
        }

        terrObjDestroyed(this);
    }
}
