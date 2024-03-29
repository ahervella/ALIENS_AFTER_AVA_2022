﻿using System.Collections;
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

    [SerializeField]
    private bool unscaledDeltaTime = false;

    private bool cachedGamePaused = false;

    private float deltaTime => unscaledDeltaTime ? UnscaledTimeIfNotPaused(cachedGamePaused) : Time.deltaTime;

    private Coroutine flashCR = null;
    private Color startClearColor;

    protected virtual void Awake()
    {
        CacheSpriteComponentRef();

        startClearColor = new Color(
            settings.FlashColor.r, settings.FlashColor.g, settings.FlashColor.b, 0);
        SetFlashColor(startClearColor);

        optionalFlashDelegate?.RegisterForDelegateInvoked(OnFlashDelegate);
    }

    //TODO: share this with the code that uses the HelperUtil's UnscaledTimeIfNotPaused
    // (Make a different universal pause? Or this good?)
    private void OnCurrGameModeChange(GameModeEnum _, GameModeEnum newMode)
    {
        cachedGamePaused = newMode == GameModeEnum.PAUSE;
    }

    protected abstract void CacheSpriteComponentRef();

    protected abstract void SetFlashColor(Color clr);

    protected abstract Color GetFlashColor();

    private bool infFlashing = false;

    protected virtual int OnFlashDelegate(bool _)
    {
        Flash();
        return 0;
    }

    public void Flash(FlashType flashType = FlashType.FLASH_LOOP)
    {
        StopInfFlashing();
        if (!isActiveAndEnabled) { return; }
            SafeStartCoroutine(ref flashCR, FlashCR(flashType), this);
    }

    public void StopInfFlashing()
    {
        infFlashing = false;
    }

    private IEnumerator FlashCR(FlashType flashType)
    {
        switch (flashType)
        {
            case FlashType.FLASH_LOOP:
                yield return FlashHalfCR(true);
                yield return FlashHalfCR(false);
                break;
            case FlashType.INF_FLASH_LOOP:
                infFlashing = true;
                while(infFlashing)
                {
                    yield return FlashHalfCR(true);
                    yield return FlashHalfCR(false);
                }
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
            perc += deltaTime / (settings.FlashLoopTime / 2);

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

public enum FlashType { FLASH_LOOP = 0, FLASH_ON = 1, FLASH_OFF = 2, INSTANT_OFF = 3, INSTANT_ON = 4, INF_FLASH_LOOP = 5 }
