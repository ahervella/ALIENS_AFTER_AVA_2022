using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarManager : MonoBehaviour
{
    [SerializeField]
    private BoolDelegateSO energyBarDisplayDelegate = null;

    [SerializeField]
    private GameObject barDisplayContainer = null;

    [SerializeField]
    private PSO_CurrentEnergy currEnergy = null;

    [SerializeField]
    private PSO_CurrentPlayerAction currAction = null;

    [SerializeField]
    private SO_EnergySettings settings = null;

    [SerializeField]
    private Image maskImg;

    /// <summary>
    /// A fraction of an energy block in bar percent that is meant
    /// to give the illusion of a smooth bar filling even though
    /// it only operates in integers
    /// </summary>
    private float blockFractionPerc;

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

        energyBarDisplayDelegate.SetInvokeMethod(SetVisibility);
        currEnergy.RegisterForPropertyChanged(OnEnergyChanged);
        currAction.RegisterForPropertyChanged(OnActionChanged);
        currEnergy.ModifyValue(settings.StartingEnergy);
        blockFractionPerc = 0;
    }

    private int SetVisibility(bool visibility)
    {
        barDisplayContainer.SetActive(visibility);
        return 0;
    }

    private void OnEnergyChanged(int oldEnergy, int newEnergy)
    {
        targetFillAmount = currEnergy.Value / (float)settings.MaxEnergy;

        //minus fractionFillPerc so that it's not part of the interpolated amount
        prevFillAmount = maskImg.fillAmount - blockFractionPerc;
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
        //Debug.Log($"CurrEnergy: {currEnergy.Value}, currFillPerc: {maskImg.fillAmount}");
    }

    private void BarTweenTick()
    {
        if (currTweenPerc >= 1) { return; }

        currTweenPerc += Time.deltaTime;

        maskImg.fillAmount =
            blockFractionPerc
            + prevFillAmount
            + (targetFillAmount - prevFillAmount)
            * HelperUtil.EasedPercent(currTweenPerc);
    }

    private void RechargeTick()
    {
        //If there is no tween, then set it directly here
        if (currTweenPerc >= 1)
        {
            maskImg.fillAmount = targetFillAmount + blockFractionPerc;
        }

        //only recharge when we are in motion
        if (!shouldRegenEnergy) { return; }

        blockFractionPerc += settings.RunRechargeRatePerSec * Time.deltaTime / settings.MaxEnergy;
        

        //Once we've passed the integer threshold, trigger the currEnergy change
        if (blockFractionPerc * settings.MaxEnergy >= 1f)
        {
            blockFractionPerc = 0;
            currEnergy.ModifyValue(1);
        }
    }
}
