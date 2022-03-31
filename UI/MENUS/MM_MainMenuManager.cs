using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System;

public class MM_MainMenuManager : A_MenuManager<MainMenuButtonEnum>
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
    private SO_DeveloperToolsSettings devToolsS = null;

    private Coroutine loopVideoFadeCR = null;
    private Coroutine titleFadeCR = null;
    private Coroutine menuButtonsFadeCR = null;

    protected override void Awake()
    {
        base.Awake();
        loopVideoScreenShotRef.enabled = false;

        AssignButtonMethods();

        ResetSequence();
        StartMainMenuSequence();
    }

    private void AssignButtonMethods()
    {
        AssignOnButtonPressedMethod(MainMenuButtonEnum.RUN, PlayGame);
    }

    private void PlayGame()
    {
        S_GameModeManager.Current.ReplaceGameModeScene(GameModeEnum.PLAY);
    }

    private void ResetSequence()
    {
        loopVideoFade.color = new Color(0, 0, 0, 1);
        titleSprite.color = new Color(1, 1, 1, 0);
        buttonGroup.ForEachButton(mb => mb.SetAlpha(0));
        MenuEnabled = false;
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
            OnButtonsFadeInComplete)); ;
    }

    private void OnButtonsFadeInComplete()
    {
        menuButtonsFadeCR = null;
        MenuEnabled = true;
    }

    private IEnumerator FadeCoroutine(float delay, float fadeTime, Action<float> setAlpha, Action finishCR)
    {
        if (devToolsS.InstantMainMenuIntro)
        {
            delay = 0.1f;
            fadeTime = 0.1f;
        }

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


public enum MainMenuButtonEnum
{
    RUN = 0, BACKPACK = 1, QUIT = 2
}

