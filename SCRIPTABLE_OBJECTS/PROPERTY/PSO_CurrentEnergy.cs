using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PSO_CurrentEnergy", menuName = "ScriptableObjects/Property/PSO_CurrentEnergy")]
public class PSO_CurrentEnergy : PSO_FillBarQuant
{
    [SerializeField]
    private SO_EnergySettings settings = null;

    [SerializeField]
    private BoolDelegateSO energyRewardedDSO = null;

    public override void ModifyValue(FillBarQuant mod)
    {
        int min = 0;
        int max = settings.MaxQuant;

        mod = new FillBarQuant(Mathf.Clamp(mod.Quant, min, max), mod.TransReached, mod.TransTime);

        SetValue(mod);
    }

    //TODO: make available in base class and apply to where it should be used
    //across all fill bar quants
    public void ModifyEnergyVal(int mod)
    {
        ModifyValue(Value.Quant + mod, false, Value.TransTime);
    }

    public void RewardPlayerEnergy(PlayerActionEnum action)
    {
        ModifyEnergyVal(settings.GetEnergyReward(action));
        energyRewardedDSO.InvokeDelegateMethod(true);
    }
}
