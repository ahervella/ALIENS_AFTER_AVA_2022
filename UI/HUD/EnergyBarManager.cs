using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class EnergyBarManager : MonoBehaviour
{
    [SerializeField]
    private PSO_CurrentEnergy currEnergy = null;

    [SerializeField]
    private PSO_CurrentPlayerAction currAction = null;

    [SerializeField]
    private SO_EnergySettings settings = null;

    private Image maskImg;

    /// <summary>
    /// A fraction of an energy block in bar percent that is meant
    /// to give the illusion of a smooth bar filling even though
    /// it only operates in integers
    /// </summary>
    private float fractionFillPerc;

    /// <summary>
    /// Target number of blocks in bar percent
    /// </summary>
    private float targetFillAmount;

    /// <summary>
    /// Previous fill amount (bar percent) at the time of energy change
    /// </summary>
    private float prevFillAmount;

    /// <summary>
    /// The current tween percent (0 = start, 1 = complete)
    /// </summary>
    private float currTweenPerc;

    /// <summary>
    /// Defines whether the energy regeneration should continue
    /// </summary>
    private bool shouldRegenEnergy = true;

    private void Awake()
    {
        maskImg = GetComponent<Image>();
        currEnergy.RegisterForPropertyChanged(OnEnergyChanged);
        currAction.RegisterForPropertyChanged(OnActionChanged);
        currEnergy.ModifyValue(settings.StartingEnergy);
        fractionFillPerc = 0;
    }


    private void OnEnergyChanged(int oldEnergy, int newEnergy)
    {
        float percent = currEnergy.Value / (float)settings.MaxEnergy;
        targetFillAmount = percent;

        //minus fractionFillPerc so that it's not part of the interpolated amount
        prevFillAmount = maskImg.fillAmount - fractionFillPerc;
        currTweenPerc = 0;
    }

    private void OnActionChanged(PlayerActionEnum oldAction, PlayerActionEnum newAction)
    {
        shouldRegenEnergy = !newAction.ToString().StartsWith("HURT") && newAction != PlayerActionEnum.NONE;
    }

    private void Update()
    {
        BarTweenTick();

        RechargeTick();
    }

    private void BarTweenTick()
    {
        if (currTweenPerc >= 1) { return; }

        currTweenPerc += Time.deltaTime;

        maskImg.fillAmount =
            fractionFillPerc
            + prevFillAmount
            + (targetFillAmount - prevFillAmount)
            * HelperUtil.EasedPercent(currTweenPerc);
    }

    private void RechargeTick()
    {
        //If there is no tween, then set it directly here
        if (currTweenPerc >= 1)
        {
            maskImg.fillAmount = targetFillAmount + fractionFillPerc;
        }

        //only recharge when we are in motion
        if (!shouldRegenEnergy) { return; }

        fractionFillPerc += settings.RunRechargeRatePerSec * Time.deltaTime / settings.MaxEnergy;

        //Once we've passed the integer threshold, trigger the currEnergy change
        if (fractionFillPerc >= 1f)
        {
            fractionFillPerc = 0;
            currEnergy.ModifyValue(1);
        }
    }
}
