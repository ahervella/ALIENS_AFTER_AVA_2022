using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static HelperUtil;

public abstract class SO_AnimationSettings<T> : ScriptableObject
{
    [SerializeField]
    private List<AnimationWrapper<T>> animWrappers = new List<AnimationWrapper<T>>();

    public AnimationWrapper<T> GetAnimationWrapper(T action)
    {
        return GetWrapperFromFunc(animWrappers, aw => aw.Action, action, LogEnum.WARNING, null);
    }
}

[Serializable]
public class AnimationWrapper<T>
{
    [SerializeField]
    private AnimationClip anim = null;
    public AnimationClip Anim => anim;

    [SerializeField]
    private SO_CameraAngle initCameraAngle = null;
    public SO_CameraAngle InitCameraAngle => initCameraAngle;

    [SerializeField]
    private T action = default;
    public T Action => action;

    [SerializeField]
    private T actionOnFinished = default;
    public T ActionOnFinished => actionOnFinished;

    [SerializeField]
    private int nextActionStartFrameIndex = 0;
    public int NextActionStartFrameIndex => nextActionStartFrameIndex;
}