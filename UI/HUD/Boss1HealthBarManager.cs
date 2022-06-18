using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1HealthBarManager : AFillBarManager<PSO_FillBarQuant, SO_BossHealthBarSettings>
{
    [SerializeField]
    private UISpriteFlasher healthBarFlasher = null;

    protected override void AfterAwake()
    {
    }

    protected override void AfterModifyCurrQuant(int oldQuant, int newQuant)
    {
        if (oldQuant > newQuant)
        {
            healthBarFlasher.Flash();
        }
    }

    protected override void AfterStart()
    {
    }
}
