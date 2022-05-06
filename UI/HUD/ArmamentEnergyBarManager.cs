using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmamentEnergyBarManager : AFillBarManager<IntPropertySO, SO_ArmamentHUDIconSettings>
{
    [SerializeField]
    private List<UISpriteFlasher> spriteFlashers = new List<UISpriteFlasher>();


    protected override void AfterAwake()
    {
    }

    protected override void AfterModifyCurrQuant(int oldQuant, int newQuant)
    {
        if (oldQuant == newQuant) { return; }
        bool reached = targetFillAmount >= 1;

        foreach (UISpriteFlasher uisf in spriteFlashers)
        {
            FlashType flashType = reached ? FlashType.FLASH_ON : FlashType.INSTANT_OFF;
            uisf.Flash(flashType);
        }
    }

    protected override void AfterStart()
    {
    }
}
