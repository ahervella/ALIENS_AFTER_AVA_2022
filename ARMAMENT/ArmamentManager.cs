using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmamentManager : MonoBehaviour
{
    //[SerializeField]
    //private SO_ArmamentSettings settings = null;

    [SerializeField]
    private PSO_CurrentEnergy currEnergy = null;

    [SerializeField]
    private PSO_CurrentLoadout currLoadout = null;

    [SerializeField]
    private DSO_UseArmament useArmamentDelegate = null;

    private Dictionary<AArmament, Coroutine> coolDownDict = new Dictionary<AArmament, Coroutine>();

    private void Awake()
    {
        InitCoolDownDict();
        useArmamentDelegate.SetInvokeMethod(TryUseArmament);
    }

    private void InitCoolDownDict()
    {
        foreach (Weapon w in currLoadout.Value.OrderedWeapons)
        {
            coolDownDict.Add(w, null);
        }

        foreach (Equipment e in currLoadout.Value.OrderedEquipments)
        {
            coolDownDict.Add(e, null);
        }
    }

    private bool TryUseArmament(AArmament armament)
    {
        if (coolDownDict[armament] != null)
        {
            return false;
        }

        if (!Try2ConsumeEnergyReq(armament.GetRequirements().EnergyBlocks))
        {
            return false;
        }

        coolDownDict[armament] = StartCoroutine(CoolDownCoroutine(armament));
        armament.UseArmament(transform);
        return true;
    }

    private bool Try2ConsumeEnergyReq(int energyRequest)
    {
        if (currEnergy.Value < energyRequest)
        {
            return false;
        }

        currEnergy.ModifyValue(-energyRequest);
        return true;
    }

    private IEnumerator CoolDownCoroutine(AArmament armament)
    {
        yield return new WaitForSeconds(armament.GetRequirements().CoolDownTime);
        coolDownDict[armament] = null;
    }
}


