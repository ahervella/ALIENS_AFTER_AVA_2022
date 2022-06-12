using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;
using System;
using Random = UnityEngine.Random;

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
    private Transform bossLaneChangeManagerTrans;

    [SerializeField]
    private int rawHeightPos = default;

    [SerializeField]
    private float rawEndDeathHeightPos = default;

    [SerializeField]
    private float deathRotQuant = 180f;

    private Shooter shooterInstance = null;

    private float cachedCannonLaneIndexPos;

    private float cachedCenterLanePos;

    private Vector3 cachedTarget;

    private void Awake()
    {
        cachedCannonLaneIndexPos = GetLaneXPosition(cannonLaneIndex, terrSettings);
        cachedCenterLanePos = GetLaneXPosition(0, terrSettings);
    }

    private void Update()
    {
        cachedTarget = new Vector3(
            cachedCannonLaneIndexPos + bossLaneChangeManagerTrans.position.x - cachedCenterLanePos,
            rawHeightPos,
            1.5f * terrSettings.TileDims.y
        );

        shootSpawnRef.LookAt(cachedTarget);
    }

    public void InstanceShooter(
        BoxColliderSP hb,
        bool rage,
        int shotCount = 1)
    {
        ShooterWrapper sw = settings.BeamShooterWrapper(rage);
        Vector3 projectilePos() => shootSpawnRef.position;

        shooterInstance = Shooter.InstantiateShooterObject(
            shootSpawnRef,
            shootSpawnRef,
            projectilePos,
            hb,
            sw,
            shotCount);
        shooterInstance.transform.localPosition = Vector3.zero;
        shooterInstance.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void CleanUpShooter()
    {
        if (shooterInstance == null) { return; }
        SafeDestroy(shooterInstance.gameObject);
    }

    public void InitDeath()
    {
        CleanUpShooter();
        StartCoroutine(DeathSpinFallCR());
    }

    private IEnumerator DeathSpinFallCR()
    {
        yield return new WaitForSeconds(Random.value * settings.DroneDeathFallRandDelayRange);

        Quaternion startRot = transform.localRotation;
        Quaternion endRot = Quaternion.Euler(0, 0, deathRotQuant);

        float startYPos = transform.localPosition.y;

        float perc = 0;
        while (perc < 1)
        {
            perc += Time.deltaTime / settings.DeathFallTime;

            float yPos = Mathf.Lerp(startYPos, rawEndDeathHeightPos, EasedPercent(perc));
            transform.localPosition = new Vector3(
                transform.localPosition.x, yPos, transform.localPosition.z);

            transform.localRotation = Quaternion.Lerp(startRot, endRot, EasedPercent(perc));

            yield return null;
        }

        SafeDestroy(gameObject);
    }
}
