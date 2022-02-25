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
public class DamageDisplayManager : MonoBehaviour
{
    [SerializeField]
    private SO_DamageDisplaySettings settings = null;

    [SerializeField]
    private IntPropertySO currLivesSO = null;

    private DamageWrapper cachedDW = null;

    private Coroutine damageAlphaCR = null;
    private Coroutine tintCR = null;

    private float currDamageTweenPerc = 0;
    private float cachedDamageAlphaMin = 0f;
    private float cachedDamageAlphaMax = 0f;
    private int flashTweenDirection = 1;

    private float currTintTweenPerc = 0;
    private float prevTintPerc = 0;

    private Image damageImg;

    private void Awake()
    {
        damageImg = GetComponent<Image>();
        currLivesSO.RegisterForPropertyChanged(OnLivesChanged);
        settings.CamTintMat.color = new Color(1, 1, 1, 0);
        OnLivesChanged(currLivesSO.Value, currLivesSO.Value);
    }

    private void OnLivesChanged(int prevLife, int newLife)
    {
        cachedDW = settings.GetDamageWrapper(newLife);

        //if just starting run
        if (damageAlphaCR == null)
        {
            cachedDamageAlphaMax = cachedDW.DamageAlphaPercentMax;
            cachedDamageAlphaMin = cachedDW.DamageAlphaPercentMin;
            StartFlashingDamageAlpha();
        }


        prevTintPerc = 1 - settings.CamTintMat.color.a;
        StartTintCoroutine();
    }

    private void StartFlashingDamageAlpha()
    {
        StopFlashingDamageAlpha();
        currDamageTweenPerc = 0f;
        damageAlphaCR = StartCoroutine(DamageFlashCoroutine());
    }

    private void StopFlashingDamageAlpha()
    {
        if (damageAlphaCR == null)
        {
            return;
        }

        StopCoroutine(damageAlphaCR);
        damageAlphaCR = null;
    }

    private IEnumerator DamageFlashCoroutine()
    {
        while (true)
        {
            currDamageTweenPerc += Time.deltaTime / (cachedDW.DamageAlphaPulseTime / 2) * flashTweenDirection;

            float alpha = Mathf.Lerp(cachedDamageAlphaMin, cachedDamageAlphaMax, EasedPercent(currDamageTweenPerc));

            damageImg.color = new Color(1, 1, 1, alpha);


            if (currDamageTweenPerc <= 0)
            {
                currDamageTweenPerc = 0;
                flashTweenDirection = 1;
                cachedDamageAlphaMax = cachedDW.DamageAlphaPercentMax;

                if (cachedDamageAlphaMax == cachedDamageAlphaMin)
                {
                    StopFlashingDamageAlpha();
                }
            }
            else if (currDamageTweenPerc >= 1)
            {
                currDamageTweenPerc = 1;
                flashTweenDirection = -1;
                cachedDamageAlphaMin = cachedDW.DamageAlphaPercentMin;

                if (cachedDamageAlphaMax == cachedDamageAlphaMin)
                {
                    StopFlashingDamageAlpha();
                }
            }

            yield return null;
        }
    }


    private void StartTintCoroutine()
    {
        StopTintCoroutine();
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
            currTintTweenPerc += Time.deltaTime / settings.TintTweenTime;

            float rgbMultiplyer = Mathf.Lerp(prevTintPerc, cachedDW.TintPercent, EasedPercent(currTintTweenPerc));

            float r = settings.HurtTintColor.r * rgbMultiplyer;
            float g = settings.HurtTintColor.g * rgbMultiplyer;
            float b = settings.HurtTintColor.b * rgbMultiplyer;
            float a = 1 - rgbMultiplyer;

            settings.CamTintMat.SetColor("_Color", new Color(r, g, b, a));

            yield return null;
        }

        StopTintCoroutine();
    }
}
