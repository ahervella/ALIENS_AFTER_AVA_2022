using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PSO_LaneChange", menuName = "ScriptableObjects/Delegates/PSO_LaneChange")]
public class PSO_LaneChange : PropertySO<LaneChange>
{
    public override void ModifyValue(LaneChange mod)
    {
        SetValue(mod);
    }
}

[Serializable]
public class LaneChange
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="movingRight">is the lane change going left or right?</param>
    /// <param name="time">time for evnironment to move</param>
    public LaneChange(bool movingRight, float time)
    {

        //Only let lane changes with a min of 0 time
        float modTime = Mathf.Max(time, 0f);

        this.dir = movingRight? 1 : -1;
        this.time = modTime;
    }

    private int dir = default;
    public int Dir => dir;

    private float time = default;
    public float Time => time;
}