using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1Animation : BaseAnimation<Boss1State>
{
    protected override void OnActionChange(Boss1State prevAction, Boss1State newAction)
    {
        AnimationClip animClip = settings.GetAnimation(newAction);
        spriteAnimator.Play(animClip);
    }
}
