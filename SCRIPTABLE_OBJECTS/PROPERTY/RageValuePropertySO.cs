using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RageValuePropertySO<T> : PropertySO<RageValue<T>>
{
    public override void ModifyValue(RageValue<T> mod)
    {
        SetValue(mod);
    }
}
