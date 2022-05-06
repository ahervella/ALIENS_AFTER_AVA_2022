using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioWrapperSource))]
public class ArmamentEnergyBarManager : AFillBarManager<IntPropertySO, SO_ArmamentHUDIconSettings>
{
    [SerializeField]
    private List<UISpriteFlasher> spriteFlashers = new List<UISpriteFlasher>();

    [SerializeField]
    private AAudioWrapperV2 armamentReadyAudio = null;

    private AudioWrapperSource audioSource;

    protected override void AfterAwake()
    {
        audioSource = GetComponent<AudioWrapperSource>();
        foreach (UISpriteFlasher uisf in spriteFlashers)
        {
            uisf.Flash(FlashType.FLASH_ON);
        }
    }

    protected override void AfterModifyCurrQuant(int oldQuant, int newQuant)
    {
        if (oldQuant == newQuant) { return; }
        if (newQuant < cachedMaxQuant)
        {
            foreach (UISpriteFlasher uisf in spriteFlashers)
            {
                uisf.Flash(FlashType.INSTANT_OFF);
            }
        }

        if (newQuant >= cachedMaxQuant)
        {
            if (oldQuant < cachedMaxQuant)
            {
                armamentReadyAudio.PlayAudioWrapper(audioSource);
            }
            foreach (UISpriteFlasher uisf in spriteFlashers)
            {
                uisf.Flash(FlashType.FLASH_ON);
            }
        }
    }

    protected override void AfterStart()
    {
    }
}
