using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//TODO: use this for the ArmamentEnergyBarManager hold flash stuff?
[RequireComponent(typeof(Image))]
public class UISpriteFlasher : ASpriteFlasher
{
    [SerializeField]
    private Image uiSpriteRef = null;

    //TODO: will also be convienent here once change everything to PSOs
    [SerializeField]
    private BoolDelegateSO barFullHoldFlashDelegate = null;

    private bool cachdedHoldFlashState = false;

    private Image flashImg;

    protected override void Awake()
    {
        base.Awake();
        barFullHoldFlashDelegate?.RegisterForDelegateInvoked(OnFullFlashDelegate);
    }

    private int OnFullFlashDelegate(bool isFull)
    {
        cachdedHoldFlashState = isFull;
        if (isFull)
        {
            Flash(FlashType.FLASH_ON);
        }
        else
        {
            Flash(FlashType.FLASH_OFF);
        }
        return 0;
    }

    protected override int OnFlashDelegate(bool _)
    {
        Flash(cachdedHoldFlashState ? FlashType.FLASH_ON : FlashType.FLASH_LOOP);
        return 0;
    }

    protected override void CacheSpriteComponentRef()
    {
        flashImg = GetComponent<Image>();
    }

    protected override Color GetFlashColor()
    {
        return flashImg.color;
    }

    protected override void SetFlashColor(Color clr)
    {
        flashImg.color = clr;
    }

    protected override void UpdateFlashSprite()
    {
        flashImg.sprite = uiSpriteRef.sprite;
    }
}
