using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;
using System;

public class Boss3CannonDrone : MonoBehaviour
{
    [SerializeField]
    private SO_Boss3Settings settings = null;

    [SerializeField]
    private SO_TerrSettings terrSettings = null;

    [SerializeField]
    private Transform shootSpawnRef = null;

    [SerializeField]
    private int cannonLaneIndex = 0;

    [SerializeField]
    private int rawHeightPos = default;

    private Shooter shooterInstance = null;

    private Vector3 cachedTarget;

    private void Awake()
    {
        ConfigurNextFireSeq(0);    
    }

    public void ConfigurNextFireSeq(int bossCenterLaneOffset)
    {
        cachedTarget = new Vector3(
            GetLaneXPosition(bossCenterLaneOffset + cannonLaneIndex, terrSettings),
            rawHeightPos,
            1.5f * terrSettings.TileDims.y
        );
    }

    private void Update()
    {
        shootSpawnRef.LookAt(cachedTarget);
    }

    public void InstanceShooter(
        Func<int> laneChangeManagerCurrLane,
        BoxColliderSP hb,
        bool rage)
    {
        ShooterWrapper sw = settings.BeamShooterWrapper(rage);
        Vector3 projectilePos() => shootSpawnRef.position;

        shooterInstance = Shooter.InstantiateShooterObject(
            shootSpawnRef,
            shootSpawnRef,
            projectilePos,
            hb,
            sw);
        shooterInstance.transform.localPosition = Vector3.zero;
        shooterInstance.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void CleanUpShooter()
    {
        if (shooterInstance == null) { return; }
        SafeDestroy(shooterInstance.gameObject);
    }
}
