using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "DSO_LaneChange", menuName = "ScriptableObjects/Delegates/DSO_LaneChange")]
public class DSO_LaneChange : DelegateSO<LaneChange, int> //int is dumby type
{
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

        this.dir = movingRight ? 1 : -1;
        this.time = modTime;
    }

    private int dir = default;
    public int Dir => dir;

    private float time = default;
    public float Time => time;
}
