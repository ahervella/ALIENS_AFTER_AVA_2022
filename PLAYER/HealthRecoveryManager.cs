﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRecoveryManager : MonoBehaviour
{

    [SerializeField]
    private IntPropertySO currLives = null;

    [SerializeField]
    private SO_PlayerRunnerSettings playerSettings = null;

    private Coroutine healCR = null;

    private void Awake()
    {
        currLives.RegisterForPropertyChanged(OnCurrLivesChange);
    }

    private void OnCurrLivesChange(int oldLives, int newLives)
    {
        Debug.Log($"New Health val: {newLives}");
        StartHealCoroutine();
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
        while (currLives.Value < currLives.MaxValue())
        {
            yield return new WaitForSeconds(playerSettings.LifeRecoveryTime);
            currLives.ModifyValue(1);
        }

        healCR = null;
    }
}