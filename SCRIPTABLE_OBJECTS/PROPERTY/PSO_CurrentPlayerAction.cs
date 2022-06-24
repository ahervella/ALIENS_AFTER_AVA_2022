using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PSO_CurrentPlayerAction", menuName = "ScriptableObjects/Property/PSO_CurrentPlayerAction")]
public class PSO_CurrentPlayerAction : PropertySO<PlayerActionEnum>
{
    [SerializeField]
    private float bufferTimeThreshold = 0.5f;
    
    [NonSerialized]
    private PlayerActionEnum bufferedAction = PlayerActionEnum.NULL;

    [NonSerialized]
    private float bufferCachedTime = -1f;

    public override void ModifyValue(PlayerActionEnum mod)
    {
        if (Value != mod)
        {
            SetValue(mod);
        }
    }

    public bool TryPerform(PlayerActionEnum action, bool permissionOverride)
    {
        if (permissionOverride)
        {
            SetValue(action);
            return true;
        }

        //TODO: just make it so we only need the permission here?
        switch (action)
        {
            case PlayerActionEnum.ROLL:
            case PlayerActionEnum.JUMP:
            if (Value == PlayerActionEnum.RUN || Value == PlayerActionEnum.LAND
                || Value == PlayerActionEnum.DODGE_R || Value == PlayerActionEnum.DODGE_L)
                {
                    ModifyValue(action);
                    return true;
                }
                break;
            case PlayerActionEnum.DODGE_L:
            case PlayerActionEnum.DODGE_R:
            case PlayerActionEnum.SPRINT:
                if (Value == PlayerActionEnum.RUN || Value == PlayerActionEnum.LAND)
                {
                    ModifyValue(action);
                    return true;
                }
                break;

            case PlayerActionEnum.LONG_JUMP:
                if (Value == PlayerActionEnum.SPRINT)
                {
                    ModifyValue(action);
                    return true;
                }
                break;
        }

        //CacheBuffer(action);
        return false;
    }

    private void CacheBuffer(PlayerActionEnum action)
    {
        bufferCachedTime = Time.time;
        bufferedAction = action;
    }

    public bool TryToUseBufferAction(float gameTime)
    {
        if (gameTime - bufferCachedTime > bufferTimeThreshold)
        {
            return false;
        }
        TryPerform(bufferedAction, false); 
        bufferedAction = PlayerActionEnum.NULL;
        bufferCachedTime = -1;
        return true;
    }

    public void PerformCorrespondingHurt(PlayerActionEnum actionRequired)
    {
        switch (actionRequired)
        {
            case PlayerActionEnum.ROLL:
            //if was hit mid air
            //case PlayerActionEnum.TAKE_DAMAGE_AIR:
                ModifyValue(PlayerActionEnum.HURT_UPPER);
                return;

            case PlayerActionEnum.JUMP:
                ModifyValue(PlayerActionEnum.HURT_LOWER);
                return;

            default:
                ModifyValue(PlayerActionEnum.HURT_CENTER);
                return;
        }
    }

    public bool IsPlayingHurtAnim()
    {
        return IsHurtAnim(Value);
    }

    public bool IsHurtAnim(PlayerActionEnum anim)
    {
        return anim == PlayerActionEnum.HURT_AIR
            || anim == PlayerActionEnum.HURT_CENTER
            || anim == PlayerActionEnum.HURT_LOWER
            || anim == PlayerActionEnum.HURT_UPPER;
    }
}
