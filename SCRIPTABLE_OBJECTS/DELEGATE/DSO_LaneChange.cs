using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DSO_LaneChange", menuName = "ScriptableObjects/Delegates/DSO_LaneChange")]
public class DSO_LaneChange : DelegateSO<LaneChange, int> //int is dumby type
{
}

public class LaneChange
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="movingRight">is the lane change going left or right?</param>
    /// <param name="time">time for evnironment to move</param>
    public LaneChange(bool movingRight, float time)
    {
        InitLaneChange(movingRight ? 1 : -1, time);
    }

    public LaneChange(int dirMag, float time)
    {
        InitLaneChange(dirMag, time);
    }

    private void InitLaneChange(int dirMag, float time)
    {
        //Only let lane changes with a min of 0 time
        float modTime = Mathf.Max(time, 0f);

        this.dirMag = dirMag;
        this.time = modTime;
    }

    private int dirMag = default;

    public int Dir => Mathf.Clamp(dirMag, -1, 1); //max is exclusive

    public int DirMag => dirMag;

    private float time = default;
    public float Time => time;
}
