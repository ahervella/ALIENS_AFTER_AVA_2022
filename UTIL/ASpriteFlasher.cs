using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HelperUtil;

public abstract class ASpriteFlasher : MonoBehaviour
{

    [SerializeField]
    private SO_SpriteFlasherSettings settings = null;

    [SerializeField]
    private BoolDelegateSO optionalFlashDelegate = null;

    private Coroutine flashCR = null;
    private Color startClearColor;

    private void Awake()
    {
        CacheSpriteComponentRef();

        startClearColor = new Color(
            settings.FlashColor.r, settings.FlashColor.g, settings.FlashColor.b, 0);
        SetFlashColor(startClearColor);

        if (optionalFlashDelegate != null)
        {
            optionalFlashDelegate.RegisterForDelegateInvoked(OnFlashDelegate);
        }
    }

    protected abstract void CacheSpriteComponentRef();

    protected abstract void SetFlashColor(Color clr);

    protected abstract Color GetFlashColor();

    private int OnFlashDelegate(bool _)
    {
        Flash();
        return 0;
    }

    public void Flash(FlashType flashType = FlashType.FLASH_LOOP)
    {
        SafeStartCoroutine(ref flashCR, FlashCR(flashType), this);
    }

    private IEnumerator FlashCR(FlashType flashType)
    {
        switch (flashType)
        {
            case FlashType.FLASH_LOOP:
                yield return FlashHalfCR(true);
                yield return FlashHalfCR(false);
                break;

            case FlashType.FLASH_ON:
                yield return FlashHalfCR(true);
                break;

            case FlashType.FLASH_OFF:
                yield return FlashHalfCR(false);
                break;

            case FlashType.INSTANT_OFF:
                SetFlashColor(startClearColor);
                break;

            case FlashType.INSTANT_ON:
                SetFlashColor(settings.FlashColor);
                break;
        }

        flashCR = null;
    }

    private IEnumerator FlashHalfCR(bool onOrOff)
    {
        //in case we are hit multiple times very quickly
        Color currStartClr = GetFlashColor();

        Color endVal = onOrOff ? settings.FlashColor : startClearColor;

        float perc = 0;
        while (perc < 1)
        {
            perc += Time.deltaTime / (settings.FlashLoopTime / 2);

            SetFlashColor(Color.Lerp(
                currStartClr, endVal, EasedPercent(perc)));

            //hack to get animation
            UpdateFlashSprite();
            yield return null;
        }
    }

    protected abstract void UpdateFlashSprite();

    private void OnDestroy()
    {
        optionalFlashDelegate?.DeRegisterFromDelegateInvoked(OnFlashDelegate);
    }
}

public enum FlashType { FLASH_LOOP, FLASH_ON, FLASH_OFF, INSTANT_OFF, INSTANT_ON }
