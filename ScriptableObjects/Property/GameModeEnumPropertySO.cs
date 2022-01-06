using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_GameModeEnumPropertySO", menuName = "ScriptableObjects/Property/GameModeEnumPropertySO", order = 2)]

public class GameModeEnumPropertySO : PropertySO<GameModeEnum>
{
    public override void ModifyValue(GameModeEnum mod)
    {
        SetValue(mod);
    }
}
