using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HelperUtil;

[RequireComponent(typeof(Image))]
public class DamageImpactManager : MonoBehaviour
{
    [SerializeField]
    private SO_DamageUISettings settings = null;

    [SerializeField]
    private IntPropertySO currLivesSO = null;

    [SerializeField]
    private PSO_CurrentGameMode currGameMode = null;

    private bool cachedGamePaused = false;

    private Image impactImg;

    private Coroutine impactCR = null;

    private void Awake()
    {
        impactImg = GetComponent<Image>();
        impactImg.color = new Color(1, 1, 1, 0);

        currGameMode.RegisterForPropertyChanged(OnGameModeChanged);
        currLivesSO.RegisterForPropertyChanged(OnLivesChanged);
        OnLivesChanged(currLivesSO.Value, currLivesSO.Value);
    }

    //TODO: Move this to helper utilities with a ref to the cached gamemode
    private void OnGameModeChanged(GameModeEnum _, GameModeEnum newMode)
    {
        cachedGamePaused = newMode == GameModeEnum.PAUSE;
    }

    private void OnLivesChanged(int prevLife, int newLife)
    {
        if (prevLife <= newLife) { return; }

        if (impactCR != null)
        {
            StopCoroutine(impactCR);
        }

        impactImg.sprite = settings.GetRandomImpactSprite();
        impactImg.color = new Color(1, 1, 1, settings.DamageImpactAlpha);

        impactCR = StartCoroutine(DamageImpactCoroutine());
    }

    private IEnumerator DamageImpactCoroutine()
    {
        while (impactImg.color.a > 0)
        {
            if (cachedGamePaused) { yield return null; }

            impactImg.color = new Color(1, 1, 1,
                impactImg.color.a - Time.unscaledDeltaTime / settings.DamageImpactTweenTime);

            yield return null;
        }

        impactCR = null;
    }
}