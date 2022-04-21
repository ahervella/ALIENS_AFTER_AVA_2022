using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static HelperUtil;
using static UnityEngine.InputSystem.InputAction;

public class EndOfDemoManager : MonoBehaviour
{
    [SerializeField]
    private SO_EndOfDemoSettings settings = null;

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    [SerializeField]
    private IntPropertySO currZone = null;

    [SerializeField]
    private SO_InputManager inputManager = null;

    [SerializeField]
    private SO_DeveloperToolsSettings devToolsSettings = null;

    [SerializeField]
    private BoolPropertySO currEndOfDemo = null;

    [SerializeField]
    private RectTransform endOfDemoContainer = null;

    [SerializeField]
    private Image fade2BlackImg = null;

    [SerializeField]
    private TextMeshProUGUI endOfDemoText = null;

    [SerializeField]
    private TextMeshProUGUI pressAnyInputText = null;

    private void Awake()
    {
        endOfDemoContainer.gameObject.SetActive(false);

        if (!devToolsSettings.DemoMode) { return; }

        fade2BlackImg.color = new Color(0, 0, 0, 0);

        Color clr = endOfDemoText.color;
        endOfDemoText.color = new Color(clr.r, clr.g, clr.b, 0);

        clr = pressAnyInputText.color;
        pressAnyInputText.color = new Color(clr.r, clr.g, clr.b, 0);

        endOfDemoText.text = settings.EndOfDemoText;
        pressAnyInputText.text = settings.PressAnyInputText;
        currZone.RegisterForPropertyChanged(OnZoneChange);
    }

    private void Start()
    {
        currEndOfDemo.ModifyValue(false);
    }

    private void OnZoneChange(int oldZone, int newZone)
    {
        if (newZone == 2)
        {
            currEndOfDemo.ModifyValue(true);
            endOfDemoContainer.gameObject.SetActive(true);
            StartCoroutine(InitEndOfDemoCR());
        }
    }

    private IEnumerator InitEndOfDemoCR()
    {
        //TODO: make this a sharedUtil thing
        float perc = 0;
        while (perc <= 1)
        {
            perc += Time.deltaTime / settings.Fade2BlackTime;
            fade2BlackImg.color = new Color(0, 0, 0, EasedPercent(perc));
            yield return null;
        }

        yield return new WaitForSeconds(settings.TextDelay);

        yield return SetTextColor(dir : 1);

        inputManager.RegisterForAnyInput(OnAnyInput);
    }

    private IEnumerator SetTextColor (float dir)
    {
        Color demoClr = endOfDemoText.color;
        Color inputClr = pressAnyInputText.color;

        float perc = dir > 0 ? 0 : 1;
        while (perc >= 0 && perc <= 1)
        {
            perc += Time.deltaTime / settings.TextFadeInTime * dir;

            endOfDemoText.color = new Color(
                demoClr.r, demoClr.g, demoClr.b, EasedPercent(perc));

            pressAnyInputText.color = new Color(
                inputClr.r, inputClr.g, inputClr.b, EasedPercent(perc));

            yield return null;
        }
    }

    private void OnAnyInput(CallbackContext ctx)
    {
        inputManager.UnregisterFromAnyInput(OnAnyInput);
        StartCoroutine(ExitEndOfDemoCR());
    }

    private IEnumerator ExitEndOfDemoCR()
    {
        yield return SetTextColor(dir : -1);

        yield return new WaitForSeconds(settings.Transition2MainMenuDelay);

        //return to zone 1 for next run
        currZone.ModifyValue(-1);

        currGameMode.ModifyValue(GameModeEnum.MAINMENU);
    }
}
