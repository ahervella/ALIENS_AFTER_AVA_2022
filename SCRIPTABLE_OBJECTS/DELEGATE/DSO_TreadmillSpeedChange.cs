using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DSO_TreadmillSpeedChange", menuName = "ScriptableObjects/Delegates/DSO_TreadmillSpeedChange")]
public class DSO_TreadmillSpeedChange : DelegateSO<TreadmillSpeedChange, int>
{
}

public class TreadmillSpeedChange
{
    private float speedPerc;
    public float SpeedPerc => speedPerc;

    private float transitionTime;
    public float TransitionTime => transitionTime;

    public TreadmillSpeedChange(float speedPerc, float transitionTime)
    {
        this.speedPerc = speedPerc;
        this.transitionTime = transitionTime;
    }
}