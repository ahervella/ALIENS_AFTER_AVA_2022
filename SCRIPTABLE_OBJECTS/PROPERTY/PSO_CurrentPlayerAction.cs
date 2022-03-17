using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PSO_CurrentPlayerAction", menuName = "ScriptableObjects/Property/PSO_CurrentPlayerAction")]
public class PSO_CurrentPlayerAction : PropertySO<PlayerActionEnum>
{
    public override void ModifyValue(PlayerActionEnum mod)
    {
        if (Value != mod)
        {
            SetValue(mod);
        }
    }

    public bool TryPerform(PlayerActionEnum action)
    {
        switch (action)
        {
            case PlayerActionEnum.DODGE_L:
            case PlayerActionEnum.DODGE_R:
            case PlayerActionEnum.ROLL:
            case PlayerActionEnum.JUMP:
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
        return false;
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
        return Value == PlayerActionEnum.HURT_AIR
            || Value == PlayerActionEnum.HURT_CENTER
            || Value == PlayerActionEnum.HURT_LOWER
            || Value == PlayerActionEnum.HURT_UPPER;
    }
}
