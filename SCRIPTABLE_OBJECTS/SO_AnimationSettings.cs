using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public abstract class SO_AnimationSettings<T> : ScriptableObject
{
    [SerializeField]
    private List<AnimationWrapper<T>> animWrappers = new List<AnimationWrapper<T>>();

    public AnimationWrapper<T> GetAnimationWrapper(T action)
    {
        //return animWrappers.FirstOrDefault(wrapper => wrapper.Action.Equals(action));
        foreach(AnimationWrapper<T> aw in animWrappers)
        {
            if (aw.Action.Equals(action))
            {
                return aw;
            }
        }

        Debug.LogError($"Could not find an animation wrapper for {action} :(");
        return null;
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