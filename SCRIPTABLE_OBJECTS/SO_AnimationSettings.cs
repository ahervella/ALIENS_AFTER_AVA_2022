using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public abstract class SO_AnimationSettings<T> : ScriptableObject
{
    [SerializeField]
    private List<AninmationWrapper<T>> animWrappers = new List<AninmationWrapper<T>>();

    public AninmationWrapper<T> GetAnimationWrapper(T action)
    {
        return animWrappers.FirstOrDefault(wrapper => wrapper.Action.Equals(action));
    }
}

[Serializable]
public class AninmationWrapper<T>
{
    [SerializeField]
    private AnimationClip anim;
    public AnimationClip Anim => anim;

    [SerializeField]
    private SO_CameraAngle initCameraAngle;
    public SO_CameraAngle InitCameraAngle => initCameraAngle;

    [SerializeField]
    private T action;
    public T Action => action;

    [SerializeField]
    private T actionOnFinished;
    public T ActionOnFinished => actionOnFinished;

    [SerializeField]
    private int nextActionStartFrameIndex = 0;
    public int NextActionStartFrameIndex => nextActionStartFrameIndex;
}