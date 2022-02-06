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
}
