using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PowerTools;
using System.Linq;

public class PlayerAnimation : BaseAnimation<PlayerActionEnum, SO_PlayerAnimationSettings>
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
    private BoolDelegateSO playerDeathTrigger = null;

    [SerializeField]
    private SO_PlayerRunnerSettings playerSettings = null;

    public bool PrematureActionChangeAllowed { get; private set; }

    protected override void OnActionChange(PlayerActionEnum prevAction, PlayerActionEnum newAction)
    {
        PrematureActionChangeAllowed = false;

        //only because they are currently the same jumping animation
        if (prevAction == PlayerActionEnum.JUMP && newAction == PlayerActionEnum.GRAPPLE_REEL)
        {
            float time = spriteAnimator.GetNormalisedTime();
            spriteAnimator.SetTime(time);
        }

        AnimationClip animClip = settings.GetAnimationAndChangeCamAngle(newAction, targetCameraAngle);
        spriteAnimator.Play(animClip);
    }

    public override void AE_OnAnimFinished()
    {
        base.AE_OnAnimFinished();

        if (((PSO_CurrentPlayerAction)currAction).TryToUseBufferAction(Time.time))
        {
            spriteAnimator.SetTime(0);
        }
    }

    public void AE_ActionChangedAllowed()
    {
        Debug.Log("premature change true");
        PrematureActionChangeAllowed = true;
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
            playerDeathTrigger.InvokeDelegateMethod(true);
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

    protected override void OnStart()
    {
    }
}
