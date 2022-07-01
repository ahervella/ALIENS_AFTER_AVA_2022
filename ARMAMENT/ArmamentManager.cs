using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmamentManager : MonoBehaviour
{
    [SerializeField]
    private SO_DeveloperToolsSettings devTools = null;

    [SerializeField]
    private PSO_CurrentEnergy currEnergy = null;

    [SerializeField]
    private PSO_CurrentLoadout currLoadout = null;

    [SerializeField]
    private PSO_CurrentPlayerAction currAction = null;

    [SerializeField]
    private DSO_UseArmament useArmamentDelegate = null;

    [SerializeField]
    private Transform projectileSpawnPoint = null;

    [SerializeField]
    private Transform playerCenterSpawnPoint = null;

    [SerializeField]
    private List<BoolPropertySO> weaponActivePSOs = new List<BoolPropertySO>();

    private void Awake()
    {
        CacheArmaments();
        useArmamentDelegate.RegisterForDelegateInvoked(TryUseArmament);

        //TODO: I know we aren't suppose to use PSOs in Awake in case
        //they are registering, but not sure how else to cache the
        //energy req PSO for AArmaments for the AFillBar to use on Start

    }

    private void CacheArmaments()
    {
        foreach (LoadoutWrapper<Weapon> w in currLoadout.Value.OrderedWeapons)
        {
            w.Armament.CacheArmamentLevelRequirements();
        }

        foreach(LoadoutWrapper<Equipment> e in currLoadout.Value.OrderedEquipments)
        {
            e.Armament.CacheArmamentLevelRequirements();
        }
    }

    private bool TryUseArmament(AArmament armament)
    {
        if (!devTools.NoReqsForArmaments)
        {
            if (armament == null) { return false; }

            if (!armament.CanSpawnAnotherInstance) { return false; }

            if (armament is Weapon && weaponActivePSOs.FindAll(pso => pso.Value).Count > 0)
            {
                return false;
            }

            if (armament.ApplicableActions.Count > 0 && !armament.ApplicableActions.Contains(currAction.Value))
            {
                Debug.Log("tried to use armament '" + armament.name + "' but was incorrect action!");
                return false;
            }

            if (!armament.ArmamentCoolDownPSO.Value.TransReached || armament.ArmamentCoolDownPSO.Value.Quant != 0)
            {
                Debug.Log("tried to use armament '" + armament.name + "' but cool down not reached!");
                return false;
            }

            if (!Try2ConsumeEnergyReq(armament.GetRequirements().EnergyBlocks))
            {
                Debug.Log("tried to use armament '" + armament.name + "' but not enough energy!");
                return false;
            }

            StartCoolDown(armament);
        }

        Transform spawnPoint = armament is Weapon ? projectileSpawnPoint : playerCenterSpawnPoint;
        armament.UseArmament(spawnPoint);
        return true;
    }

    private bool Try2ConsumeEnergyReq(int energyRequest)
    {
        if (currEnergy.Value.Quant < energyRequest
            || (currEnergy.Value.Quant == energyRequest && !currEnergy.Value.TransReached))
        {
            return false;
        }

        currEnergy.ModifyEnergyVal(-energyRequest);
        return true;
    }

    private void StartCoolDown(AArmament armament)
    {
        armament.ArmamentCoolDownPSO.ModifyValue(1, false, 0);
        armament.ArmamentCoolDownPSO.ModifyValue(0, false, armament.GetRequirements().CoolDownTime);
    }
}


