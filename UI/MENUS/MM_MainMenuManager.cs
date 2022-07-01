using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(AudioWrapperSource))]
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
    private PSO_CurrentGameMode currGameMode = null;

    [SerializeField]
    private PSO_CurrentTutorialMode currTutMode = null;

    [SerializeField]
    private PSO_CurrentZonePhase currZonePhase = null;

    [SerializeField]
    private IntPropertySO currZone = null;

    [SerializeField]
    private SO_DeveloperToolsSettings devToolsS = null;

    [SerializeField]
    private BoolPropertySO firstTimePlayingPSO = null;

    [SerializeField]
    private BoolPropertySO lvlSelectLockedPSO = null;

    [SerializeField]
    private GameObject mainSubButtonGroup = null;

    [SerializeField]
    private GameObject levelSelectSubButtonGroup = null;

    private Coroutine loopVideoFadeCR = null;
    private Coroutine titleFadeCR = null;
    private Coroutine menuButtonsFadeCR = null;

    private AudioWrapperSource audioSource;

    protected override void OnMenuAwake()
    {
        audioSource = GetComponent<AudioWrapperSource>();

        loopVideoScreenShotRef.enabled = false;

        AssignButtonMethods();
        
        levelSelectSubButtonGroup.SetActive(false);
        mainSubButtonGroup.SetActive(true);

        buttonGroup.GetButton(MainMenuButtonEnum.SELECT_LVL).ButtonEnabled = !lvlSelectLockedPSO.Value;

        ResetSequence();
        StartMainMenuSequence();

        currZonePhase.ModifyValue(ZonePhaseEnum.NO_BOSS_SUB1);
    }

    protected override void OnMenuStart() { }

    //TODO: make this part of awake, make sure all menus account for this change
    private void AssignButtonMethods()
    {
        AssignOnButtonPressedMethod(MainMenuButtonEnum.RUN, PlayGame);
        AssignOnButtonPressedMethod(MainMenuButtonEnum.QUIT, QuitGameApplication);
        AssignOnButtonPressedMethod(MainMenuButtonEnum.BACKPACK, OpenBackpack);
        AssignOnButtonPressedMethod(MainMenuButtonEnum.SELECT_LVL, SelectLevelMenu);
        AssignOnButtonPressedMethod(MainMenuButtonEnum.BACK_TO_MAIN, Return2MainSubMenu);
        AssignOnButtonPressedMethod(MainMenuButtonEnum.LVL_1, RunLvl1);
        AssignOnButtonPressedMethod(MainMenuButtonEnum.LVL_2, RunLvl2);
        AssignOnButtonPressedMethod(MainMenuButtonEnum.LVL_3, RunLvl3);
    }

    private void PlayGame()
    {
        currZonePhase.ModifyValue(ZonePhaseEnum.ZONE_INTRO_TRANS);
        
        if (firstTimePlayingPSO.Value)
        {
            currTutMode.ModifyValue(TutorialModeEnum.FIRST_RUN);
            currGameMode.ModifyValue(GameModeEnum.TUTORIAL);
            return;
        }
        currGameMode.ModifyValue(GameModeEnum.PLAY);
    }

    private void SelectLevelMenu()
    {
        levelSelectSubButtonGroup.SetActive(true);
        mainSubButtonGroup.SetActive(false);
        SelectButton(buttonGroup.GetButton(MainMenuButtonEnum.BACK_TO_MAIN));
    }

    private void Return2MainSubMenu()
    {
        levelSelectSubButtonGroup.SetActive(false);
        mainSubButtonGroup.SetActive(true);
        SelectButton(buttonGroup.GetButton(MainMenuButtonEnum.SELECT_LVL));
    }

    private void RunLvl1() => RunSelectedLevel(1);
    private void RunLvl2() => RunSelectedLevel(2);
    private void RunLvl3() => RunSelectedLevel(3);

    private void RunSelectedLevel(int lvl)
    {
        currZone.DirectlySetValue(lvl);
        currZonePhase.ModifyValue(ZonePhaseEnum.ZONE_INTRO_TRANS);
        currGameMode.ModifyValue(GameModeEnum.PLAY);
    }

    private void QuitGameApplication()
    {
        currGameMode.ModifyValue(GameModeEnum.QUIT);
    }

    private void OpenBackpack()
    {
        currGameMode.ModifyValue(GameModeEnum.BACKPACK);
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
        //settings.MainMenuAudio.PlayAudioWrapper(audioSource);

        MainMenuTimingWrapper wrapper = settings.GetTimingWrapper();

        loopVideoFadeCR = StartCoroutine(FadeCoroutine(
            wrapper.VideoDelayFromBlack,
            wrapper.VideoFadeInTime,
            a => loopVideoFade.color = new Color(0, 0, 0, 1 - a),
            () => loopVideoFadeCR = null));

        titleFadeCR = StartCoroutine(FadeCoroutine(
            wrapper.TitleTotalDelay,
            wrapper.TitleFadeInTime,
            a => titleSprite.color = new Color(1, 1, 1, a),
            () => titleFadeCR = null));

        menuButtonsFadeCR = StartCoroutine(FadeCoroutine(
            wrapper.ButtonPromptTotalDelay,
            wrapper.ButtonPromptFadeInTime,
            a => buttonGroup.ForEachButton(mb => mb.SetAlpha(a)),
            OnButtonsFadeInComplete)); ;
    }

    private void OnButtonsFadeInComplete()
    {
        menuButtonsFadeCR = null;
        MenuEnabled = true;
        StartCoroutine(MainMenuVoxLoop());
    }

    private IEnumerator MainMenuVoxLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(settings.GetRandVoxDelay());
            settings.MainMenuVox.PlayAudioWrapper(audioSource);
        }
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
    RUN = 0,
    BACKPACK = 1,
    QUIT = 2,
    SELECT_LVL = 3,
    BACK_TO_MAIN = 7,
    LVL_1 = 4,
    LVL_2 = 5,
    LVL_3 = 6 
}

