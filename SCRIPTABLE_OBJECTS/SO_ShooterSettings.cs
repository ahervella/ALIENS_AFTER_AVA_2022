using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

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
        private GameObject weaponPrefab;
        public GameObject WeaponPrefab => weaponPrefab;

        [SerializeField]
        private int randWeight = 1;
        public int RandWeight => randWeight;

        [SerializeField]
        private List<ShooterZoneWrapper> shooterWrappers = new List<ShooterZoneWrapper>();

        public ShooterWrapper GetShooterWrapper(IntPropertySO currZone)
        {
            foreach(ShooterZoneWrapper szw in shooterWrappers)
            {
                if (szw.Zone == currZone.Value)
                {
                    return new ShooterWrapper(weaponPrefab, szw.DelayTime);
                }
            }

            Debug.LogError("Something went wrong with the shooter zone wrapper :(");
            return null;
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
    public ShooterWrapper(GameObject weaponPrefab, float delayTime)
    {
        this.weaponPrefab = weaponPrefab;
        this.delayTime = delayTime;
    }

    private GameObject weaponPrefab;
    public GameObject WeaponPrefab => weaponPrefab;

    private float delayTime = 2f;
    public float DelayTime => delayTime;
}
