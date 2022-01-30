using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PSO_LaneChange : PropertySO<LaneChange>
{
    public override void ModifyValue(LaneChange mod)
    {
        SetValue(new LaneChange(mod.Dir, mod.Time));
    }
}

[Serializable]
public class LaneChange
{
    public LaneChange(int dir, float time)
    {

        //Only let lane changes happen one at a time, with a min of 0 time
        int modDir = Mathf.Clamp(dir, -1, 1);
        float modTime = Mathf.Min(time, 0f);

        Dir = modDir;
        Time = modTime;
    }

    public int Dir { get; private set; }
    public float Time { get; private set; }
}