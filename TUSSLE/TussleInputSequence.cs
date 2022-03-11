using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class TussleInputSequence : MonoBehaviour
{
    [SerializeField]
    private SO_InputManager inputManager = null;

    [SerializeField]
    private SO_TussleSettings settings = null;

    [SerializeField]
    private List<TussleButton> orderedButtons = new List<TussleButton>();

    private Coroutine sequenceCR = null;
    private Coroutine failTimerCR = null;
    private InputEnum currInputRequirement;
    private bool waitingForCorrectInput = true;
    Action<bool> finishCallback;
    public void StartSequence(Action<bool> finishCallback)
    {
        //callback to trigger whether sequence was a success or failure
        this.finishCallback = finishCallback;
        sequenceCR = StartCoroutine(SequenceCoroutine());
    }

    private IEnumerator SequenceCoroutine()
    {
        RegisterForPossibleInputs();

        failTimerCR = StartCoroutine(FailTimer(settings.GetCurrZoneTussleTime()));

        foreach (TussleButton button in orderedButtons)
        {
            waitingForCorrectInput = true;
            currInputRequirement = button.AssignedInput;
            button.SetButtonState(TussleButtonState.PRESS_NOW);

            while (waitingForCorrectInput)
            {
                yield return null;
            }

            button.SetButtonState(TussleButtonState.SUCCESS);
        }

        StopCoroutine(failTimerCR);
        finishCallback(true);
    }

    private void RegisterForPossibleInputs()
    {
        foreach(InputEnum i in settings.GetListOfAvailableInputs())
        {
            inputManager.RegisterForInput(i, () => OnInput(i));
        }
    }

    private IEnumerator FailTimer(float time)
    {
        yield return new WaitForSeconds(time);
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
        finishCallback(false);
    }
}
