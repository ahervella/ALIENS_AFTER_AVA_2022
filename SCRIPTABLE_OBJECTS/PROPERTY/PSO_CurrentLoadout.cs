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
    public Loadout(List<Weapon> orderedWeapons, List<Equipment> orderedEquipments)
    {
        this.orderedWeapons = orderedWeapons;
        this.orderedEquipments = orderedEquipments;
    }

    [SerializeField]
    private List<Weapon> orderedWeapons = new List<Weapon>();
    public List<Weapon> OrderedWeapons => orderedWeapons;

    [SerializeField]
    private List<Equipment> orderedEquipments = new List<Equipment>();
    public List<Equipment> OrderedEquipments => orderedEquipments;
}