using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static HelperUtil;

public abstract class SO_AnimationSettings<T> : ScriptableObject
{
    [SerializeField]
    private List<AnimationWrapper<T>> animWrappers = new List<AnimationWrapper<T>>();

    public AnimationClip GetAnimation(T action)
    {
        return GetAnimationAndChangeCamAngle(action, null);
    }

    public AnimationClip GetAnimationAndChangeCamAngle(T action, PSO_TargetCameraAngle targetCamAngle)
    {
        AnimationWrapper<T> wrapper = GetAnimationWrapper(action);
        if (wrapper == null) { return null; }

        if (wrapper.InitCameraAngle != null)
        {
            targetCamAngle?.ModifyValue(wrapper.InitCameraAngle);
        }
        if (wrapper.Anim == null)
        {
            Debug.LogError($"No animation clip set for animation setting: {name} -> {action}");
        }

        return wrapper.Anim;
    }

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