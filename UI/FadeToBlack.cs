using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static HelperUtil;

//TODO: implement this across the game where we are fading in UI, way better!
public class FadeToBlack : MonoBehaviour
{
    [SerializeField]
    private Image fadeImg = null;

    private Coroutine fadeCR = null;

    private bool firstFade = true;

    public void InitFade(bool fadeInOrOut, float fadeTime, float delay, float holdTime, Action onFinish)
    {
        SafeStartCoroutine(ref fadeCR, FadeCR(fadeInOrOut, fadeTime, delay, holdTime, onFinish), this);
    }

    private IEnumerator FadeCR(bool fadeInOrOut, float fadeTime, float delay, float holdTime, Action onFinish)
    {
        float startAlpha = firstFade ? (fadeInOrOut ? 1 : 0) : fadeImg.color.a;
        float endAlpha = fadeInOrOut ? 0 : 1;
        float perc = 0;

        firstFade = false;

        fadeImg.gameObject.SetActive(true);
        fadeImg.color = new Color(0, 0, 0, startAlpha);

        yield return new WaitForSeconds(delay);

        while (perc < 1)
        {
            perc += Time.deltaTime / fadeTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, perc);
            fadeImg.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        if (fadeInOrOut)
        {
            fadeImg.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(holdTime);

        onFinish?.Invoke();
    }
}
