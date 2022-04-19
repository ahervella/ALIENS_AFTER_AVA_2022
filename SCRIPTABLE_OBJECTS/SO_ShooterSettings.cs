using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using static HelperUtil;

[CreateAssetMenu(fileName = "SO_ShooterSettings", menuName = "ScriptableObjects/StaticData/SO_ShooterSettings")]
public class SO_ShooterSettings : ScriptableObject
{
    [SerializeField]
    private List<ShooterWeaponZoneWrapper> shooterWeaponWrappers = new List<ShooterWeaponZoneWrapper>();

    [SerializeField]
    private IntPropertySO currZone = null;

    public ShooterWrapper GetRandShooterWrapper()
    {
        int totalWeight = 0;
        foreach (ShooterWeaponZoneWrapper sw in shooterWeaponWrappers)
        {
            totalWeight += sw.RandWeight;
        }

        int randSelection = Random.Range(0, totalWeight);
        foreach (ShooterWeaponZoneWrapper sw in shooterWeaponWrappers)
        {

            totalWeight -= sw.RandWeight;
            if (totalWeight <= randSelection)
            {
                return sw.GetShooterWrapper(currZone);
            }
        }

        Debug.LogError("Something went wrong with the weights in the shooter :(");
        return null;
    }


    [Serializable]
    private class ShooterWeaponZoneWrapper
    {
        [SerializeField]
        private WeaponFire weaponFirePrefab;
        public WeaponFire WeaponFirePrefab => weaponFirePrefab;

        [SerializeField]
        private int randWeight = 1;
        public int RandWeight => randWeight;

        [SerializeField]
        private List<ShooterZoneWrapper> shooterWrappers = new List<ShooterZoneWrapper>();

        public ShooterWrapper GetShooterWrapper(IntPropertySO currZone)
        {
            ShooterZoneWrapper wrapper = GetWrapperFromFunc(shooterWrappers, szw => szw.Zone, currZone.Value, LogEnum.ERROR, null,
                "Something went wrong with the shooter zone wrapper :(");

            return wrapper == null ? null : new ShooterWrapper(weaponFirePrefab, wrapper.DelayTime);
        }
    }


    [Serializable]
    private class ShooterZoneWrapper
    {
        [SerializeField]
        private int zone;
        public int Zone => zone;

        [SerializeField]
        private float delayTime = 2f;
        public float DelayTime => delayTime;
    }
}


public class ShooterWrapper
{
    public ShooterWrapper(WeaponFire weaponFirePrefab, float delayTime)
    {
        this.weaponFirePrefab = weaponFirePrefab;
        this.delayTime = delayTime;
    }

    private WeaponFire weaponFirePrefab;
    public WeaponFire WeaponFirePrefab => weaponFirePrefab;

    private float delayTime = 2f;
    public float DelayTime => delayTime;
}
