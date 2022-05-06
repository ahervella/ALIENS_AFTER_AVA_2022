using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarManager : AFillBarManager<PSO_CurrentEnergy, SO_EnergySettings>
{
    [SerializeField]
    private BoolDelegateSO energyBarDisplayDelegate = null;

    [SerializeField]
    private PSO_CurrentPlayerAction currAction = null;


    protected override void AfterAwake()
    {
        energyBarDisplayDelegate.RegisterForDelegateInvoked(SetVisibility);
        currAction.RegisterForPropertyChanged(OnActionChanged);
    }

    protected override void AfterStart()
    {
        OnActionChanged(currAction.Value, currAction.Value);
    }

    protected override void AfterModifyCurrQuant(int oldQuant, int newQuant)
    {
    }

    private void OnActionChanged(PlayerActionEnum oldAction, PlayerActionEnum newAction)
    {
        bool shouldRegenEnergy = !newAction.ToString().StartsWith("HURT") && newAction != PlayerActionEnum.NONE;
        autoDeltaRatePerSec = shouldRegenEnergy ? settings.RunRechargeRatePerSec : 0;
    }
}
