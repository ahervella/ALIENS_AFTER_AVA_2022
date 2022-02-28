using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ESO_", menuName = "ScriptableObjects/Armaments/Equipment")]
public class Equipment : AArmament
{
    [SerializeField]
    private EquipmentEnum equipmentType = EquipmentEnum.NONE;
    public EquipmentEnum EquipmentType => equipmentType;
}
