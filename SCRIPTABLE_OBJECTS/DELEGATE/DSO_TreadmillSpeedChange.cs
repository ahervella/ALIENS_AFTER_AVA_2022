using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "DSO_TreadmillSpeedChange", menuName = "ScriptableObjects/Delegates/DSO_TreadmillSpeedChange")]
public class DSO_TreadmillSpeedChange : DelegateSO<TreadmillSpeedChange, int>
{
}

[Serializable]
public class TreadmillSpeedChange
{
    [SerializeField]
    private float speedPerc;
    public float SpeedPerc => speedPerc;

    [SerializeField]
    private float transitionTime;
    public float TransitionTime => transitionTime;

    public TreadmillSpeedChange(float speedPerc, float transitionTime)
    {
        this.speedPerc = speedPerc;
        this.transitionTime = transitionTime;
    }
}