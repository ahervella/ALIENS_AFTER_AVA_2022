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
    private BoolPropertySO grappleOnFlag = null;

    [SerializeField]
    private DSO_UseArmament useArmamentDelegate = null;

    [SerializeField]
    private Transform projectileSpawnPoint = null;

    [SerializeField]
    private Transform playerCenterSpawnPoint = null;

    private Dictionary<AArmament, Tuple<Coroutine, IntPropertySO>> coolDownCRDict = new Dictionary<AArmament, Tuple<Coroutine, IntPropertySO>>();

    private void Awake()
    {
        CacheArmaments();
        InitCoolDownDict();
        useArmamentDelegate.RegisterForDelegateInvoked(TryUseArmament);

        //TODO: I know we aren't suppose to use PSOs in Awake in case
        //they are registering, but not sure how else to cache the
        //energy req PSO for AArmaments for the AFillBar to use on Start

    }

    private void CacheArmaments()
    {
        foreach (Weapon w in currLoadout.Value.OrderedWeapons)
        {
            w?.CacheArmamentLevelRequirements();
        }

        foreach(Equipment e in currLoadout.Value.OrderedEquipments)
        {
            e?.CacheArmamentLevelRequirements();
        }
    }

    private void InitCoolDownDict()
    {
        foreach (Weapon w in currLoadout.Value.OrderedWeapons)
        {
            if (w == null) { continue; }
            coolDownCRDict.Add(w, new Tuple<Coroutine, IntPropertySO>(null, w.ArmamentCoolDownPSO));
        }

        foreach (Equipment e in currLoadout.Value.OrderedEquipments)
        {
            if (e == null) { continue; }
            coolDownCRDict.Add(e, new Tuple<Coroutine, IntPropertySO>(null, e.ArmamentCoolDownPSO));
        }
    }

    private bool TryUseArmament(AArmament armament)
    {
        if (armament == null) { return false; }

        if (grappleOnFlag.Value)
        {
            return false;
        }

        if (coolDownCRDict[armament].Item1 != null)
        {
            return false;
        }

        if (!Try2ConsumeEnergyReq(armament.GetRequirements().EnergyBlocks))
        {
            return false;
        }

        coolDownCRDict[armament] = new Tuple<Coroutine, IntPropertySO>(
            StartCoroutine(CoolDownCoroutine(armament, coolDownCRDict[armament].Item2)),
            coolDownCRDict[armament].Item2);

        Transform spawnPoint = armament is Weapon ? projectileSpawnPoint : playerCenterSpawnPoint;
        armament.UseArmament(spawnPoint);
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

    private IEnumerator CoolDownCoroutine(AArmament armament, IntPropertySO coolDownPSO)
    {
        coolDownPSO.ModifyValue(-coolDownPSO.Value);

        float coolDownTime = armament.GetRequirements().CoolDownTime;
        float currTime = 0;
        while (currTime < coolDownTime)
        {
            currTime += Time.deltaTime;
            coolDownPSO.ModifyValueNoInvoke((int)Mathf.Floor(Time.deltaTime / coolDownTime * 100f));
            yield return null;
        }

        coolDownPSO.ModifyValue(-coolDownPSO.Value + 100);

        coolDownCRDict[armament] = new Tuple<Coroutine, IntPropertySO>(null, armament.ArmamentCoolDownPSO);
    }
}


