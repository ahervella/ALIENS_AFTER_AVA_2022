using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static HelperUtil;

public class MM_DemoMenuManager : A_MenuManager<DemoMenuEnum>
{
    [SerializeField]
    private SO_EndOfDemoSettings settings = null;

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    [SerializeField]
    private SO_DeveloperToolsSettings devToolsSettings = null;

    [SerializeField]
    private List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();


    protected override void OnMenuAwake()
    {
        if (!devToolsSettings.DemoMode)
        {
            currGameMode.ModifyValue(GameModeEnum.MAINMENU);
            return;
        }

        foreach (TextMeshProUGUI t in texts)
        {
            Color clr = t.color;
            t.color = new Color(clr.r, clr.g, clr.b, 0);
        }

        AssignButtonMethods();

        MenuEnabled = false;

         StartCoroutine(InitEndOfDemoCR());
    }

    private IEnumerator InitEndOfDemoCR()
    {
        yield return SetTextColorCR(dir: 1);
    }

    private IEnumerator SetTextColorCR(float dir)
    {
        List<Color> clrs = new List<Color>();

        foreach (TextMeshProUGUI t in texts)
        {
            clrs.Add(t.color);
        }

        float perc = dir > 0 ? 0 : 1;
        while (perc >= 0 && perc <= 1)
        {
            perc += Time.deltaTime / settings.TextFadeInTime * dir;

            for (int i = 0; i < clrs.Count; i++)
            {
                texts[i].color = new Color(
                    clrs[i].r, clrs[i].g, clrs[i].b, EasedPercent(perc));
            }

            yield return null;
        }

        if (dir > 0)
        {
            MenuEnabled = true;
        }
    }

    
    private void AssignButtonMethods()
    {
        AssignOnButtonPressedMethod(DemoMenuEnum.DEMO_LINK, GoToDemoLink);
        AssignOnButtonPressedMethod(DemoMenuEnum.MAINMENU, GoToMainMenu);
    }

    private void GoToDemoLink()
    {
        Application.OpenURL(settings.URL);
    }

    private void GoToMainMenu()
    {
        ExitEndOfDemo();
        MenuEnabled = false;
    }

    public void ExitEndOfDemo()
    {
        StartCoroutine(ExitEndOfDemoCR());
    }

    private IEnumerator ExitEndOfDemoCR()
    {
        yield return SetTextColorCR(dir: -1);

        yield return new WaitForSeconds(settings.Transition2MainMenuDelay);

        currGameMode.ModifyValue(GameModeEnum.MAINMENU);
    }

    protected override void OnMenuStart()
    {
    }
}

public enum DemoMenuEnum { DEMO_LINK = 0, MAINMENU = 1}
