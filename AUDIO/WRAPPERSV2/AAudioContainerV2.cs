using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AAudioContainerV2 : AAudioWrapperV2
{
    [SerializeField]
    [Range(-60f, 0f)]
    protected float levelOffsetDb = 0;

    public float LevelOffsetDb { get; set; }

    public void OnEnable()
    {
        ResetLevelOffset();
    }

    override protected void ResetLevelOffset()
    {
        LevelOffsetDb = levelOffsetDb;
    }
}
