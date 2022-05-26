﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class HitBoxWrapper
{
    public HitBoxWrapper(int minXRange, int maxXRange, PlayerActionEnum customAvoidAction)
    {
        this.minXRange = minXRange;
        this.maxXRange = maxXRange;
        this.customAvoidAction = customAvoidAction;
    }

    [SerializeField]
    private int minXRange = 0;
    public int MinXRange => minXRange;

    [SerializeField]
    private int maxXRange = 1;
    public int MaxXRange => maxXRange;

    [SerializeField]
    private PlayerActionEnum customAvoidAction = PlayerActionEnum.NONE;
    public PlayerActionEnum CustomAvoidAction => customAvoidAction;

    //[SerializeField]
    //private PlayerActionEnum customTakeDownAction = PlayerActionEnum.NULL;
    //public PlayerActionEnum CustomTakeDownAction => customTakeDownAction;

    public BoxColliderSP InstancedHB { private set; get; }

    public void CacheInstancedHB(BoxColliderSP hb)
    {
        InstancedHB = hb;
    }
}
