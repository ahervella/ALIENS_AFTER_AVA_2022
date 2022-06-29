using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using static HelperUtil;

public class MM_TutorialMenuManager : A_MenuManager<TutorialMenuEnum>
{
    [SerializeField]
    private SO_TutorialMenuSettings settings = null;

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    [SerializeField]
    private PSO_CurrentTutorialMode currTutMode = null;

    [SerializeField]
    List<TutorialModeWrapper> tutorialWrappers = new List<TutorialModeWrapper>();

    //TODO: move this wrapper stuff to the settings SO and switch to instancing
    //prefabs instead of having to populate the manager and inspector
    [Serializable]
    private class TutorialModeWrapper
    {
        [SerializeField]
        private TutorialModeEnum mode = default;
        public TutorialModeEnum Mode => mode;

        [SerializeField]
        private BoolPropertySO oneShotPSO = null;
        public BoolPropertySO OneShotPSO => oneShotPSO;

        [SerializeField]
        private List<RectTransform> orderedSlides = new List<RectTransform>();
        public List<RectTransform> OrderedSlides => orderedSlides;

        [SerializeField]
        private string lastSlideButtonText = string.Empty;
        public string LastSlideButtonText => lastSlideButtonText;

        [SerializeField]
        private GameModeEnum lastSlideButtonGameMode = default;
        public GameModeEnum LastSlideButtonGameMode => lastSlideButtonGameMode;
    }

    [SerializeField]
    private Image slidesBlackFade = null;

    [SerializeField]
    private Image menuBlackFade = null;

    [SerializeField]
    private TextMeshProUGUI lastWords = null;

    private Coroutine slidesFadeCR = null;

    private int currSlideIndex = 0;

    private string ogNextButtonText;

    private MenuButton backButton;
    private MenuButton nextButton;

    private TutorialModeWrapper cachedTutWrapper;
    private int TotalSlides => cachedTutWrapper.OrderedSlides.Count;


    protected override void OnMenuAwake()
    {
        cachedTutWrapper = GetWrapperFromFunc(
            tutorialWrappers,
            tw => tw.Mode,
            currTutMode.Value,
            LogEnum.ERROR,
            null);

        nextButton = buttonGroup.GetButton(TutorialMenuEnum.NEXT);
        ogNextButtonText = nextButton.GetText();

        backButton = buttonGroup.GetButton(TutorialMenuEnum.BACK);

        SetBackButton();
        SetNextButton();

        AssignOnButtonPressedMethod(TutorialMenuEnum.BACK, OnBackButton);
        AssignOnButtonPressedMethod(TutorialMenuEnum.NEXT, OnNextButton);

        SetSlide();

        lastWords.text = settings.LastWordsText;

        slidesBlackFade.color = new Color(0, 0, 0, 0);

        menuBlackFade.color = Color.black;
        StartCoroutine(BlackFade(menuBlackFade, true));

    }

    protected override void OnMenuStart()
    {
        cachedTutWrapper.OneShotPSO.ModifyValue(false);
    }

    private void OnBackButton()
    {
        currSlideIndex--;
        SafeStartCoroutine(ref slidesFadeCR, NewSlideTransition(), this);
        SetBackButton();
    }

    private void SetBackButton()
    {
        if (TotalSlides == 1)
        {
            backButton.ButtonEnabled = false;
            backButton.gameObject.SetActive(false);
            return;
        }

        if (currSlideIndex == 0)
        {
            backButton.ButtonEnabled = false;
            SelectButton(nextButton);
        }

        if (currSlideIndex == TotalSlides - 2)
        {
            nextButton.SetText(ogNextButtonText);
        }
    }

    private void OnNextButton()
    {
        currSlideIndex++;
        SafeStartCoroutine(ref slidesFadeCR, NewSlideTransition(), this);
        SetNextButton();
    }

    private void SetNextButton()
    {
        if (currSlideIndex == 1)
        {
            backButton.ButtonEnabled = true;
        }

        if (currSlideIndex == TotalSlides - 1)
        {
            nextButton.SetText(cachedTutWrapper.LastSlideButtonText);
        }

        else if (currSlideIndex == TotalSlides)
        {
            MenuEnabled = false;
            StartCoroutine(BlackFade(
                menuBlackFade, false, () => StartCoroutine(LastWordsCR())));
        }
    }


    private IEnumerator NewSlideTransition()
    {
        yield return BlackFade(slidesBlackFade, false);

        SetSlide();

        if (currSlideIndex == TotalSlides)
        {
            slidesFadeCR = null;
            yield break;
        }

        yield return BlackFade(slidesBlackFade, true);

        slidesFadeCR = null;
    }

    private IEnumerator BlackFade(Image blackFade, bool fadeInOrOut, Action endMethod = null)
    {
        float startingVal = fadeInOrOut ? 1 : blackFade.color.a;
        float endVal = fadeInOrOut ? 0 : 1;
        float perc = 0;
        while (perc < 1)
        {
            perc += Time.deltaTime / (settings.TotalFadeTime / 2f);
            float alpha = Mathf.Lerp(startingVal, endVal, EasedPercent(perc));
            blackFade.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        endMethod?.Invoke();
    }

    private void SetSlide()
    {
        for (int i = 0; i < TotalSlides; i++)
        {
            if (i == currSlideIndex)
            {
                cachedTutWrapper.OrderedSlides[i].gameObject.SetActive(true);
                continue;
            }

            cachedTutWrapper.OrderedSlides[i].gameObject.SetActive(false);
        }
    }

    private IEnumerator LastWordsCR()
    {
        if (cachedTutWrapper.Mode == TutorialModeEnum.FIRST_RUN)
        {
            yield return new WaitForSeconds(settings.LastWordsDelay);
            lastWords.gameObject.SetActive(true);
            yield return new WaitForSeconds(settings.LastWordsTime);
            lastWords.gameObject.SetActive(false);
            yield return new WaitForSeconds(settings.LastWordsAfterDelay);
        }
        else
        {
            yield return new WaitForSeconds(settings.PostTutorialTransDelay);
        }

        currGameMode.ModifyValue(cachedTutWrapper.LastSlideButtonGameMode);
    }
}

public enum TutorialMenuEnum
{
    BACK = 0,
    NEXT = 1,
    DEMO_FEEDBACK_LINK = 2
}

public enum TutorialModeEnum
{
    FIRST_RUN = 0,
    GRAPPLE_TUSSLE = 1,
    ATOM_CANNON = 2,
    LVL_SELECT = 3,
    NONE = 4
}
