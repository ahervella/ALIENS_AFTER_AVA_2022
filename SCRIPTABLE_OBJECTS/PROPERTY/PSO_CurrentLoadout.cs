using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PSO_CurrentLoadout", menuName = "ScriptableObjects/Property/PSO_CurrentLoadout")]
public class PSO_CurrentLoadout : PropertySO<Loadout>
{
    public override void ModifyValue(Loadout newLoadout)
    {
        SetValue(newLoadout);
    }
}

[Serializable]
public class Loadout : AGameSaveData
{
    public Loadout(List<LoadoutWrapper<Weapon>> orderedWeapons, List<LoadoutWrapper<Equipment>> orderedEquipments)
    {
        this.orderedWeapons = orderedWeapons;
        this.orderedEquipments = orderedEquipments;
    }

    [SerializeField]
    private List<LoadoutWrapper<Weapon>> orderedWeapons = new List<LoadoutWrapper<Weapon>>();
    public List<LoadoutWrapper<Weapon>> OrderedWeapons => orderedWeapons;

    [SerializeField]
    private List<LoadoutWrapper<Equipment>> orderedEquipments = new List<LoadoutWrapper<Equipment>>();
    public List<LoadoutWrapper<Equipment>> OrderedEquipments => orderedEquipments;
}

[Serializable]
public class LoadoutWrapper<T> where T : AArmament
{
    [SerializeField]
    private T armament = null;
    public T Armament => armament;

    [SerializeField]
    private BoolPropertySO lockedPSO = null;
    public BoolPropertySO LockedPSO => lockedPSO;
}