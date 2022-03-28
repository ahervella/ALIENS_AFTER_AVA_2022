using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PowerTools;
using System.Linq;

public class PlayerAnimation : BaseAnimation<PlayerActionEnum>
{
    [SerializeField]
    private PSO_TargetCameraAngle targetCameraAngle = null;

    [SerializeField]
    private IntPropertySO currLives = null;

    [SerializeField]
    private DSO_TreadmillSpeedChange treadmillToggleDelegate = null;

    [SerializeField]
    private DSO_LaneChange laneChangeDelegate = null;

    [SerializeField]
    private SO_PlayerRunnerSettings playerSettings = null;

    protected override void OnPlayerActionChange(PlayerActionEnum prevAction, PlayerActionEnum newAction)
    {
        AnimationWrapper<PlayerActionEnum> paw = settings.GetAnimationWrapper(newAction);
        if (paw != null)
        {
            if (paw.InitCameraAngle != null)
            {
                targetCameraAngle.ModifyValue(paw.InitCameraAngle);
            }

            spriteAnimator.Play(paw.Anim);
        }
    }

    public void AE_TriggerActionCameraAngle(SO_CameraAngle ca)
    {
        targetCameraAngle.ModifyValue(ca);
    }

    public void AE_DeathFrame()
    {
        if (currLives.Value <= currLives.MinValue())
        {
            spriteAnimator.Stop();
        }
    }

    public void AE_LaneChange(int dir)
    {
        laneChangeDelegate.InvokeDelegateMethod(new LaneChange(dir > 0, playerSettings.LaneChangeTime));
    }

    public void AE_ResumeTreadmill(float transitionTime)
    {
        treadmillToggleDelegate.InvokeDelegateMethod(new TreadmillSpeedChange(1, transitionTime));
    }

    public void AE_PauseTreadmill(float transitionTime)
    {
        treadmillToggleDelegate.InvokeDelegateMethod(new TreadmillSpeedChange(0, transitionTime));
    }
}
