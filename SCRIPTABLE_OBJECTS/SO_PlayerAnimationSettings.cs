using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[CreateAssetMenu(fileName = "SO_PlayerAnimationSettings", menuName = "ScriptableObjects/StaticData/SO_PlayerAnimationSettings")]
public class SO_PlayerAnimationSettings : ScriptableObject
{
    [SerializeField]
    private List<PlayerAnimationWrapper> animWrappers = new List<PlayerAnimationWrapper>();

    public PlayerAnimationWrapper GetAnimationWrapper(PlayerActionEnum action)
    {
        return animWrappers.First(wrapper => wrapper.Action == action);
    }
}

[Serializable]
public class PlayerAnimationWrapper
{
    [SerializeField]
    private AnimationClip anim;
    public AnimationClip Anim => anim;

    [SerializeField]
    private SO_CameraAngle initCameraAngle;
    public SO_CameraAngle InitCameraAngle => initCameraAngle;

    [SerializeField]
    private PlayerActionEnum action;
    public PlayerActionEnum Action => action;
}