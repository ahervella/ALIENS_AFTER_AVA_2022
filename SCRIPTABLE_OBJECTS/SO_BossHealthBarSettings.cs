using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_BossHealthBarSettings", menuName = "ScriptableObjects/StaticData/SO_BossHealthBarSettings")]
public class SO_BossHealthBarSettings : SO_AFillBarSettings
{
    [SerializeField]
    private SO_ABossSettings bossSettings = null;

    public override int MaxQuant => bossSettings.StartingHealth;

    public override int StartingQuant => bossSettings.StartingHealth;
}
