using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PSO_CurrentEnergy", menuName = "ScriptableObjects/Property/PSO_CurrentEnergy")]
public class PSO_CurrentEnergy : IntPropertySO
{
    [SerializeField]
    private SO_EnergySettings settings = null;

    public override void ModifyValue(int change)
    {
        int min = 0;
        int max = settings.MaxEnergy;

        SetValue(Mathf.Clamp(Value + change, min, max));
    }

    public void RewardPlayerEnergy(PlayerActionEnum action)
    {
        ModifyValue(settings.GetEnergyReward(action));
    }

    public bool TryConsumeWeaponEnergy(WeaponEnum weapon)
    {
        int energyReq = settings.GetWeaponEnergyReq(weapon);
        if (Value < energyReq)
        {
            return false;
        }

        ModifyValue(-energyReq);
        return true;
    }
}
