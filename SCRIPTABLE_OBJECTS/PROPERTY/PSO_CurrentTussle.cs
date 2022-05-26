using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PSO_CurrentTussle", menuName = "ScriptableObjects/Property/PSO_CurrentTussle")]
public class PSO_CurrentTussle : PropertySO<TussleWrapper>
{
    public override void ModifyValue(TussleWrapper mod)
    {
        SetValue(mod);
    }
}

public class TussleWrapper
{
    public bool PlayerAdvantage { private set; get; }
    public bool BossTussle { private set; get; }

    public TussleWrapper(bool playerAdvantage, bool bossTussle)
    {
        PlayerAdvantage = playerAdvantage;
        BossTussle = bossTussle;
    }
}
