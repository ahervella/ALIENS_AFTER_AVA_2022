using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static HelperUtil;

[CreateAssetMenu(fileName = "SO_DamageQuantSettings", menuName = "ScriptableObjects/StaticData/SO_DamageQuantSettings")]
public class SO_DamageQuantSettings : ScriptableObject
{
    [SerializeField]
    private List<WeaponDamageWrapper> weaponDamageWrappers = new List<WeaponDamageWrapper>();

    [SerializeField]
    private int defaultHazardDamage = 1;

    [SerializeField]
    private DamageWrapper tussleDamage = default;

    public int GetTussleDamage(bool damage2PlayerOrAlien)
    {
        return damage2PlayerOrAlien ? tussleDamage.Damage2Player : tussleDamage.Damage2Alien;
    }

    public int GetDefaultHazardDamage() => defaultHazardDamage;

    public int GetWeaponDamage(WeaponEnum weaponType, bool damage2PlayerOrAlien)
    {
        WeaponDamageWrapper wrapper =
            GetWrapperFromFunc(weaponDamageWrappers, wdw => wdw.WeaponType, weaponType, LogEnum.ERROR, null);

        if (wrapper == null)
        {
            return 0;
        }

        return damage2PlayerOrAlien ? wrapper.DamageWrapper.Damage2Player : wrapper.DamageWrapper.Damage2Alien;
    }

    [Serializable]
    private class DamageWrapper
    {
        [SerializeField]
        private int damage2Player = default;
        public int Damage2Player => damage2Player;

        [SerializeField]
        private int damage2Alien = default;
        public int Damage2Alien => damage2Alien;
    }

    [Serializable]
    private class WeaponDamageWrapper
    {
        [SerializeField]
        private WeaponEnum weaponType = WeaponEnum.NONE;
        public WeaponEnum WeaponType => weaponType;

        [SerializeField]
        private DamageWrapper damageWrapper = default;
        public DamageWrapper DamageWrapper => damageWrapper;
    }
}
