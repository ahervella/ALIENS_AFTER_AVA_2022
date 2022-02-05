using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PowerTools;
using System.Linq;

[RequireComponent(typeof(SpriteAnim))]
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField]
    private PSO_CurrentPlayerAction currAction = null;

    [SerializeField]
    private PSO_TargetCameraAngle targetCameraAngle = null;

    [SerializeField]
    private SO_PlayerAnimationSettings settings = null;

    private SpriteAnim animator;

    private void Awake()
    {
        currAction.RegisterForPropertyChanged(OnPlayerActionChange);
        animator = GetComponent<SpriteAnim>();
    }

    private void OnPlayerActionChange(PlayerActionEnum prevAction, PlayerActionEnum newAction)
    {
        PlayerAnimationWrapper paw = settings.GetAnimationWrapper(newAction);
        if (paw != null)
        {
            if (paw.InitCameraAngle != null)
            {
                targetCameraAngle.ModifyValue(paw.InitCameraAngle);
            }

            animator.Play(paw.Anim);
        }
    }

    public void AE_OnAnimFinished()
    {
        switch (currAction.Value)
        {
            case PlayerActionEnum.DODGE_L:
            case PlayerActionEnum.DODGE_R:
            case PlayerActionEnum.FALL:
            case PlayerActionEnum.LJ_FALL:
            case PlayerActionEnum.ROLL:
            case PlayerActionEnum.SPRINT:
            case PlayerActionEnum.HURT_AIR:
            case PlayerActionEnum.HURT_CENTER:
            case PlayerActionEnum.HURT_LOWER:
            case PlayerActionEnum.HURT_UPPER:
                currAction.ModifyValue(PlayerActionEnum.RUN);
                break;

            case PlayerActionEnum.JUMP:
                currAction.ModifyValue(PlayerActionEnum.FALL);
                break;

            case PlayerActionEnum.LONG_JUMP:
                currAction.ModifyValue(PlayerActionEnum.LJ_FALL);
                break;
        }
    }

    public void AE_TriggerActionCameraAngle(SO_CameraAngle ca)
    {
        targetCameraAngle.ModifyValue(ca);
    }
}
