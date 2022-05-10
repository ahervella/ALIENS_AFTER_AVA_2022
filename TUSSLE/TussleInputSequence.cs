using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static SO_InputManager;
using static UnityEngine.InputSystem.InputAction;

public class TussleInputSequence : MonoBehaviour
{
    [SerializeField]
    private SO_InputManager inputManager = null;

    [SerializeField]
    private SO_TussleSettings settings = null;

    [SerializeField]
    private List<TussleButton> orderedButtons = new List<TussleButton>();

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    private Coroutine sequenceCR = null;

    private Coroutine failTimerCR = null;

    private InputEnum currInputRequirement;

    private bool waitingForCorrectInput = true;

    private Action<bool> finishCallback;

    private bool gamePaused => currGameMode.Value == GameModeEnum.PAUSE;

    //Used to store method reference for unregistering from inputs
    private Dictionary<InputEnum, Action<CallbackContext>> inputCallbackDict = new Dictionary<InputEnum, Action<CallbackContext>>();

    public void StartSequence(Action<bool> finishCallback)
    {
        //callback to trigger whether sequence was a success or failure
        this.finishCallback = finishCallback;
        sequenceCR = StartCoroutine(SequenceCoroutine());
    }

    private IEnumerator SequenceCoroutine()
    {
        RegisterInputChange(true);

        failTimerCR = StartCoroutine(FailTimer(settings.GetCurrZoneTussleTime()));

        foreach (TussleButton button in orderedButtons)
        {
            waitingForCorrectInput = true;
            currInputRequirement = button.AssignedInput;
            button.SetButtonState(TussleButtonStateEnum.PRESS_NOW);

            while (waitingForCorrectInput)
            {
                yield return null;
            }

            button.SetButtonState(TussleButtonStateEnum.SUCCESS);
        }

        ResolveSequence(true);
    }

    //public so we can use it with the tussle tester
    public void ResolveSequence(bool won)
    {
        if (failTimerCR != null) { StopCoroutine(failTimerCR); }
        RegisterInputChange(false);
        finishCallback(won);
    }

    private void RegisterInputChange(bool register)
    {
        foreach(InputEnum i in settings.GetListOfAvailableInputs())
        {
            if (register)
            {
                inputCallbackDict.Add(i, ctx => OnInput(i));
                inputManager.RegisterForInput(i, inputCallbackDict[i]);
            }
            else
            {
                inputManager.UnregisterFromInput(i, inputCallbackDict[i]);
            }
        }
    }

    private IEnumerator FailTimer(float time)
    {
        float currTime = 0;
        while (currTime < time)
        {
            if (!gamePaused)
            {
                currTime += Time.unscaledDeltaTime;
            }
            yield return null;
        }

        FailedInputSequence();
    }

    public void OnInput(InputEnum input)
    {
        if (currInputRequirement == input)
        {
            waitingForCorrectInput = false;
            return;
        }

        FailedInputSequence();
    }

    private void FailedInputSequence()
    {
        StopCoroutine(sequenceCR);
        foreach (TussleButton button in orderedButtons)
        {
            button.SetToFailed();
        }

        ResolveSequence(false);
    }
}
