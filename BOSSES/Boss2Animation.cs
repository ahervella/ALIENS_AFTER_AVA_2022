using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Animation : BaseAnimation<Boss2State, SO_Boss2AnimationSettings>
{
    [SerializeField]
    private float flybyCenterXOffset = 1f;

    private Vector3 startScale;
    private Vector3 startLocalPos;

    protected override void OnActionChange(Boss2State prevAction, Boss2State newAction)
    {
        AnimationClip animClip = settings.GetAnimation(newAction);
        spriteAnimator.NormalizedTime = 0;
        spriteAnimator.Play(animClip);

        switch (newAction)
        {
            case Boss2State.ATTACK_START_RIGHT:
            case Boss2State.IDLE_FLY_RIGHT:
                transform.localScale = new Vector3(-startScale.x, startScale.y, startScale.z);
                break;

            //To keep the scale from the attack start
            case Boss2State.SPREAD_WINGS_MIDDLE_LOW:
            case Boss2State.SPREAD_WINGS_MIDDLE_HIGH:
            case Boss2State.SPREAD_WINGS:
                break;

            default:
                transform.localScale = new Vector3(startScale.x, startScale.y, startScale.z);
                break;
        }

        
        switch (newAction)
        {
            case Boss2State.ATTACK_START_LEFT:
            case Boss2State.IDLE_FLY_LEFT:
                transform.localPosition = startLocalPos + new Vector3(flybyCenterXOffset, 0, 0);
                break;

            case Boss2State.ATTACK_START_RIGHT:
            case Boss2State.IDLE_FLY_RIGHT:
                transform.localPosition = startLocalPos - new Vector3(flybyCenterXOffset, 0, 0);
                break;

            default:
                transform.localPosition = startLocalPos;
                break;
        }
    }

    protected override void OnAwake()
    {
        startScale = transform.localScale;
        startLocalPos = transform.localPosition;
    }
}
