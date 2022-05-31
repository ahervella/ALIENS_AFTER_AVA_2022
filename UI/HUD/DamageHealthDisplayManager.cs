//TODO:
//This artists was nice enough to share his damage effect for free!
//We can at least credit him later (see the read me file in the ART_FILES
//folder of this repo)
//https://www.deviantart.com/xiuyanderekun/art/MMD-Bloody-Screen-Effect-DL-858960325

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HelperUtil;

[RequireComponent(typeof(Image))]
public class DamageHealthDisplayManager : MonoBehaviour
{
    [SerializeField]
    private SO_DamageUISettings settings = null;

    [SerializeField]
    private IntPropertySO currLivesSO = null;

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    private bool cachedGamePaused = false;

    private DamageWrapper cachedDW = null;

    private Coroutine damageAlphaCR = null;
    private Coroutine tintCR = null;

    private float currDamageTweenPerc = 0;
    private float cachedDamageAlphaMin = 0f;
    private float cachedDamageAlphaMax = 0f;
    private int flashTweenDirection;

    private float currTintTweenPerc = 0;
    private float prevTintPerc = 0;

    private Image damageImg;

    private void Awake()
    {
        damageImg = GetComponent<Image>();
        SetHurtTintVisiblePerc(0);
        damageImg.color = new Color(1, 1, 1, 0);
        currLivesSO.RegisterForPropertyChanged(OnLivesChanged);
        currGameMode.RegisterForPropertyChanged(OnGameModeChanged);
    }

    //TODO: Move this to helper utilities with a ref to the cached gamemode
    private void OnGameModeChanged(GameModeEnum _, GameModeEnum newMode)
    {
        cachedGamePaused = newMode == GameModeEnum.PAUSE;
    }

    private void Start()
    {
        damageImg.color = new Color(1, 1, 1, 0);
    }

    private void OnLivesChanged(int prevLife, int newLife)
    {
        cachedDW = settings.GetDamageWrapper(newLife);

        StartFlashingDamageSprite();
        StartTintCoroutine();
    }


    private void StartFlashingDamageSprite()
    {
        //if in progress, it will finish on its own when
        if (damageAlphaCR != null) { return; }

        StopFlashingDamageSprite();
        currDamageTweenPerc = 1f;
        flashTweenDirection = 1;
        damageAlphaCR = StartCoroutine(DamageSpriteFlashCoroutine());
    }

    private void StopFlashingDamageSprite()
    {
        if (damageAlphaCR == null)
        {
            return;
        }

        StopCoroutine(damageAlphaCR);
        damageAlphaCR = null;
    }

    private IEnumerator DamageSpriteFlashCoroutine()
    {
        while (cachedDamageAlphaMin != cachedDamageAlphaMax
            || cachedDW.DamageAlphaPercentMin != cachedDW.DamageAlphaPercentMax)
        {
            if (cachedGamePaused) { yield return null; }

            currDamageTweenPerc += Time.unscaledDeltaTime / (cachedDW.DamageAlphaPulseTime / 2) * flashTweenDirection;

            float alpha = Mathf.Lerp(cachedDamageAlphaMin, cachedDamageAlphaMax, EasedPercent(currDamageTweenPerc));

            damageImg.color = new Color(1, 1, 1, alpha);

            //when reached end of either swing...
            if (currDamageTweenPerc <= 0)
            {
                currDamageTweenPerc = 0;
                flashTweenDirection = 1;

                //instead when lives change, change the cached values at the end of each swing
                //to keep constant smooth transitions
                cachedDamageAlphaMax = cachedDW.DamageAlphaPercentMax;
            }
            else if (currDamageTweenPerc >= 1)
            {
                currDamageTweenPerc = 1;
                flashTweenDirection = -1;
                cachedDamageAlphaMin = cachedDW.DamageAlphaPercentMin;
            }

            yield return null;
        }

        StopFlashingDamageSprite();
    }


    //The following is just for the red tint that does not flash multiple times

    private void StartTintCoroutine()
    {
        StopTintCoroutine();
        prevTintPerc = 1 - settings.CamTintMat.color.a;
        currTintTweenPerc = 0;
        tintCR = StartCoroutine(TintCoroutine());
    }

    private void StopTintCoroutine()
    {
        if (tintCR == null)
        {
            return;
        }

        StopCoroutine(tintCR);
        tintCR = null;
    }

    private IEnumerator TintCoroutine()
    {
        while (currTintTweenPerc < 1)
        {
            if (cachedGamePaused) { yield return null; }

            currTintTweenPerc += Time.unscaledDeltaTime / settings.TintTweenTime;

            float visiblePerc = Mathf.Lerp(prevTintPerc, cachedDW.TintPercent, EasedPercent(currTintTweenPerc));

            SetHurtTintVisiblePerc(visiblePerc);

            yield return null;
        }

        StopTintCoroutine();
    }

    private void SetHurtTintVisiblePerc(float perc)
    {
        float r = settings.HurtTintColor.r * perc;
        float g = settings.HurtTintColor.g * perc;
        float b = settings.HurtTintColor.b * perc;
        float a = 1 - perc;

        settings.CamTintMat.SetColor("_Color", new Color(r, g, b, a));
    }

    private void OnDestroy()
    {
        SetHurtTintVisiblePerc(0);
    }
}
