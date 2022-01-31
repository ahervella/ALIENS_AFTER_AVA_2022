using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PSO_LaneChange", menuName = "ScriptableObjects/Delegates/PSO_LaneChange")]
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

        dir = modDir;
        time = modTime;
    }

    [SerializeField]
    private int dir;
    public int Dir => dir;

    [SerializeField]
    private float time;
    public float Time => time;
}