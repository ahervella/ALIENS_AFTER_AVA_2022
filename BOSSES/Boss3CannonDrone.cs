using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

public class Boss3CannonDrone : MonoBehaviour
{
    [SerializeField]
    private Transform shootSpawnRef = null;

    [SerializeField]
    private int cannonLaneIndex = 0;

    private Shooter shooterInstance = null;

    public void InstanceShooter(
        Transform terrHorizTrans,
        int laneChangeManagerCurrLane,
        SO_TerrSettings terrSettings,
        BoxColliderSP hb,
        ShooterWrapper sw)
    {
        Vector3 projectilePos() => new Vector3(
            GetLaneXPosition(cannonLaneIndex + laneChangeManagerCurrLane, terrSettings),
            0,
            shootSpawnRef.position.z);

        shooterInstance = Shooter.InstantiateShooterObject(
            terrHorizTrans,
            shootSpawnRef,
            projectilePos,
            hb,
            sw);
    }

    public void CleanUpShooter()
    {
        if (shooterInstance == null) { return; }
        SafeDestroy(shooterInstance.gameObject);
    }
}
