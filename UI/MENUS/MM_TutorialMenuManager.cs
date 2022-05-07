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
    List<RectTransform> orderedSlides = new List<RectTransform>();

    [SerializeField]
    private Image slidesBlackFade = null;

    [SerializeField]
    private Image menuBlackFade = null;

    [SerializeField]
    private TextMeshProUGUI lastWords = null;

    [SerializeField]
    private BoolPropertySO firstTimePlayingPSO = null;

    private Coroutine slidesFadeCR = null;

    private int currSlideIndex = 0;

    private string ogNextButtonText;

    private MenuButton backButton;
    private MenuButton nextButton;


    protected override void OnMenuAwake()
    {
        nextButton = buttonGroup.GetButton(TutorialMenuEnum.NEXT);
        ogNextButtonText = nextButton.GetText();

        backButton = buttonGroup.GetButton(TutorialMenuEnum.BACK);
        backButton.ButtonEnabled = false;

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
        firstTimePlayingPSO.ModifyValue(false);
    }

    private void OnBackButton()
    {
        currSlideIndex--;
        SafeStartCoroutine(ref slidesFadeCR, NewSlideTransition(), this);
        if (currSlideIndex == 0)
        {
            backButton.ButtonEnabled = false;
        }

        else if (currSlideIndex == orderedSlides.Count - 2)
        {
            nextButton.SetText(ogNextButtonText);
        }
    }

    private void OnNextButton()
    {
        currSlideIndex++;
        SafeStartCoroutine(ref slidesFadeCR, NewSlideTransition(), this);
        if (currSlideIndex == 1)
        {
            backButton.ButtonEnabled = true;
        }

        else if (currSlideIndex == orderedSlides.Count - 1)
        {
            nextButton.SetText(settings.LastSlideButtonText);
        }

        else if (currSlideIndex == orderedSlides.Count)
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

        if (currSlideIndex == orderedSlides.Count)
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
        for (int i = 0; i < orderedSlides.Count; i++)
        {
            if (i == currSlideIndex)
            {
                orderedSlides[i].gameObject.SetActive(true);
                continue;
            }

            orderedSlides[i].gameObject.SetActive(false);
        }
    }

    private IEnumerator LastWordsCR()
    {
        yield return new WaitForSeconds(settings.LastWordsDelay);
        lastWords.gameObject.SetActive(true);
        yield return new WaitForSeconds(settings.LastWordsTime);
        lastWords.gameObject.SetActive(false);
        yield return new WaitForSeconds(settings.LastWordsAfterDelay);
        currGameMode.ModifyValue(GameModeEnum.PLAY);
    }
}

public enum TutorialMenuEnum { BACK = 0, NEXT = 1 }
