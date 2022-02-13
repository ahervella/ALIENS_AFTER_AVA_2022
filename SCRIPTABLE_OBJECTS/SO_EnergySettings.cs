using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SO_EnergySettings", menuName = "ScriptableObjects/StaticData/SO_EnergySettings")]
public class SO_EnergySettings : ScriptableObject
{
    [SerializeField]
    private int maxEnergy = default;
    public int MaxEnergy => maxEnergy;

    [SerializeField]
    private int startingEnergy;
    public int StartingEnergy => startingEnergy;

    [SerializeField]
    private float runRechargeRatePerSec = default;
    public float RunRechargeRatePerSec => runRechargeRatePerSec;

    [SerializeField]
    private List<PlayerEnergyWrapper> playerEnergyRewards = new List<PlayerEnergyWrapper>();

    [Serializable]
    private class PlayerEnergyWrapper
    {
        [SerializeField]
        private PlayerActionEnum action;
        public PlayerActionEnum Action => action;

        [SerializeField]
        private int energy;
        public int Energy => energy;
    }

    [SerializeField]
    private List<WeaponEnergyWrapper> weaponEnergyReqs = new List<WeaponEnergyWrapper>();

    [Serializable]
    private class WeaponEnergyWrapper
    {
        [SerializeField]
        private WeaponEnum weapon;
        public WeaponEnum Weapon => weapon;

        [SerializeField]
        private int energy;
        public int Energy => energy;
    }

    public int GetEnergyReward(PlayerActionEnum action)
    {
        foreach (PlayerEnergyWrapper pew in playerEnergyRewards)
        {
            if (pew.Action == action)
            {
                return pew.Energy;
            }
        }

        Debug.LogError($"No energy quantitfy found for action {action}");
        return -1;
    }

    public int GetWeaponEnergyReq(WeaponEnum weapon)
    {
        foreach (WeaponEnergyWrapper wew in weaponEnergyReqs)
        {
            if (wew.Weapon == weapon)
            {
                return wew.Energy;
            }
        }

        Debug.LogError($"No energy quantitfy found for weapon {weapon}");
        return -1;
    }
}
