using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    private SO_MainMenuSettings settings = null;

    [SerializeField]
    private VideoPlayer loopVideo = null;

    [SerializeField]
    private Image loopVideoFade = null;

    [SerializeField]
    private Image loopVideoScreenShotRef = null;

    [SerializeField]
    private Image titleSprite = null;

    [SerializeField]
    private MenuButtonGroup<MainMenuButtonEnum> buttonGroup = null;

    private enum MainMenuButtonEnum
    {
        RUN = 0, BACKPACK = 1, QUIT = 2
    }

    [SerializeField]
    private SO_InputManager inputManager = null;

    private Coroutine loopVideoFadeCR = null;
    private Coroutine titleFadeCR = null;
    private Coroutine menuButtonsFadeCR = null;

    private void Awake()
    {
        loopVideoScreenShotRef.enabled = false;
        inputManager.EnsureIsEnabled();
    }

    //to let Buttons run their awake first
    private void Start()
    {
        ResetAll();
        StartMainMenuSequence();
    }

    private void ResetAll()
    {
        loopVideoFade.color = new Color(0, 0, 0, 1);
        titleSprite.color = new Color(1, 1, 1, 0);
        buttonGroup.GetMenuButton(MainMenuButtonEnum.RUN).OnSelect();
        buttonGroup.ForEachButton(mb => mb.SetAlpha(0));
    }

    private void StartMainMenuSequence()
    {
        loopVideoFadeCR = StartCoroutine(FadeCoroutine(
            settings.VideoDelayFromBlack,
            settings.VideoFadeInTime,
            a => loopVideoFade.color = new Color(0, 0, 0, 1 - a),
            () => loopVideoFadeCR = null));

        titleFadeCR = StartCoroutine(FadeCoroutine(
            settings.TitleTotalDelay,
            settings.TitleFadeInTime,
            a => titleSprite.color = new Color(1, 1, 1, a),
            () => titleFadeCR = null));

        menuButtonsFadeCR = StartCoroutine(FadeCoroutine(
            settings.ButtonPromptTotalDelay,
            settings.ButtonPromptFadeInTime,
            a => buttonGroup.ForEachButton(mb => mb.SetAlpha(a)),
            () => menuButtonsFadeCR = null)); ;
    }

    private IEnumerator FadeCoroutine(float delay, float fadeTime, Action<float> setAlpha, Action finishCR)
    {
        yield return new WaitForSeconds(delay);

        float timer = 0;
        while(timer < fadeTime)
        {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(0, 1, timer / fadeTime);
            setAlpha(a);
            yield return null;
        }

        finishCR();
    }
}


