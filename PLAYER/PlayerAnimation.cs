using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PowerTools;
using System.Linq;

[RequireComponent(typeof(SpriteAnim))]
public class PlayerAnimation : BaseAnimation<PlayerActionEnum>
{
    [SerializeField]
    private PSO_TargetCameraAngle targetCameraAngle = null;

    private void Awake()
    {
        currAction.RegisterForPropertyChanged(OnPlayerActionChange);
        spriteAnimator = GetComponent<SpriteAnim>();
        //included component if SpriteAnim is also included
        animator = GetComponent<Animator>();
    }

    protected override void OnPlayerActionChange(PlayerActionEnum prevAction, PlayerActionEnum newAction)
    {
        AninmationWrapper<PlayerActionEnum> paw = settings.GetAnimationWrapper(newAction);
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
}
