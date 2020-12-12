using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AAudioContainer : AAudioWrapper
{
    [SerializeField]
    [Range(-60f, 0f)]
    protected float levelOffsetDb = 0;

    public float LevelOffsetDb { get; set; }

    override protected void ResetLevelOffset()
    {
        LevelOffsetDb = levelOffsetDb;
    }
}
