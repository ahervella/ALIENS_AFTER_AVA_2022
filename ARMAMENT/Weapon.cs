using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WSO_", menuName = "ScriptableObjects/Armaments/Weapon")]
public class Weapon : AArmament
{
    [SerializeField]
    private WeaponEnum weaponType = WeaponEnum.NONE;
    public WeaponEnum WeaponType => weaponType;
}
