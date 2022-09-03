using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRecoveryManager : MonoBehaviour
{

    [SerializeField]
    private IntPropertySO currLives = null;

    [SerializeField]
    private SO_PlayerRunnerSettings playerSettings = null;

    [SerializeField]
    private SO_DeveloperToolsSettings devToolsSettings = null;

    private Coroutine healCR = null;

    //TODO reset when tussle is done, and pause when it happens?

    private void Start()
    {
        currLives.ResetToStart();
        currLives.RegisterForPropertyChanged(OnCurrLivesChange);
    
        if (devToolsSettings.StartWith1Life)
        {
            currLives.ModifyValue(-currLives.Value + 1);
        }
    }

    private void OnCurrLivesChange(int oldLives, int newLives)
    {
        Debug.Log($"New Health val: {newLives}");
        if (newLives < oldLives)
        {
            StartHealCoroutine(); ;
        }
    }

    private void StartHealCoroutine()
    {
        if (healCR != null)
        {
            StopCoroutine(healCR);
        }

        healCR = StartCoroutine(HealDamageCoroutine());
    }

    private IEnumerator HealDamageCoroutine()
    {
        while (currLives.Value < currLives.MaxValue() && currLives.Value > 0)
        {
            yield return new WaitForSeconds(playerSettings.LifeRecoveryTime);
            currLives.ModifyValue(1);
        }

        healCR = null;
    }
}
