using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PSO_CurrentTutorialMode", menuName = "ScriptableObjects/Property/PSO_CurrentTutorialMode")]
public class PSO_CurrentTutorialMode : PropertySO<TutorialModeEnum>
{
    public override void ModifyValue(TutorialModeEnum mod)
    {
        SetValue(mod);
    }
}
