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

    private Image impactImg;

    private Coroutine impactCR = null;

    private void Awake()
    {
        impactImg = GetComponent<Image>();
        impactImg.color = new Color(1, 1, 1, 0);

        currLivesSO.RegisterForPropertyChanged(OnLivesChanged);
        OnLivesChanged(currLivesSO.Value, currLivesSO.Value);
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
            impactImg.color = new Color(1, 1, 1, impactImg.color.a - Time.deltaTime / settings.DamageImpactTweenTime);

            yield return null;
        }

        impactCR = null;
    }
}