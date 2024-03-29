﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static HelperUtil;

[CreateAssetMenu(fileName = "SO_EnergySettings", menuName = "ScriptableObjects/StaticData/SO_EnergySettings")]
public class SO_EnergySettings : SO_AFillBarSettings
{
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

    public int GetEnergyReward(PlayerActionEnum action)
    {
        PlayerEnergyWrapper wrapper = GetWrapperFromFunc( playerEnergyRewards, pew => pew.Action, action, LogEnum.WARNING, null);
        return wrapper == null? 0 : wrapper.Energy;
    }
}
