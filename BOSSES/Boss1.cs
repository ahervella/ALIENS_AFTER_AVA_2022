using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

public class Boss1 : AAlienBoss<Boss1State, SO_Boss1Settings>
{
    [SerializeField]
    private DSO_LaneChange laneChangeDelegate = null;

    private List<Shooter> spawnedBulletShooters = new List<Shooter>();

    private Coroutine idleFloatCR = null;
    private Coroutine shootCR = null;

    protected override void InitDeath()
    {
        SafeStopCoroutine(ref idleFloatCR, this);
        StopShooting();
        currState.ModifyValue(Boss1State.DEATH);
    }

    protected override void InitRage()
    {
        throw new System.NotImplementedException();
    }

    protected override void SetStartingPosition()
    {
        float x = terrSettings.LaneCount / 2f * terrSettings.TileDims.x;
        float z = settings.SpawnTileRowsAway * terrSettings.TileDims.y;

        transform.position = new Vector3(x, transform.position.y, z);
    }

    protected override void OnBossAwake()
    {
        currState.RegisterForPropertyChanged(OnStateChanged);
        laneChangeDelegate.RegisterForDelegateInvoked(OnLaneChange);
        SpawnSequence();
        //currState.ModifyValue(BOSS1_ACTION.START);
    }

    private void OnStateChanged(Boss1State oldState, Boss1State newState)
    {
        switch (newState)
        {
            case Boss1State.IDLE:
                StartIdleFloatLoop();
                break;

            case Boss1State.SHOOT:
                Shoot();
                break;
        }
    }

    private void StartIdleFloatLoop()
    {
        SafeStartCoroutine(ref idleFloatCR, IdleFloatCR(), this);
    }

    private IEnumerator IdleFloatCR()
    {
        yield return new WaitForSeconds(settings.GetRandRangeShootPhaseTime(Rage));
        currState.ModifyValue(Boss1State.SHOOT);
    }

    private void Shoot()
    {
        SafeStartCoroutine(ref shootCR, ShootCR(), this);
    }

    private IEnumerator ShootCR()
    {
        //TODO: maybe in the future can randomize the guns / types of bullets
        //the boss can shoot with this!
        ShooterWrapper sw = settings.ShootSettings(Rage).GetRandShooterWrapper();

        yield return new WaitForSeconds(sw.DelayTime);

        StartFiringBullets(sw);

        yield return new WaitForSeconds(sw.DelayTime * settings.BulletsPerShot(Rage));

        currState.ModifyValue(Boss1State.SHOOT_END);
        StopShooting();
    }

    private void StopShooting()
    {
        foreach(Shooter s in spawnedBulletShooters)
        {
            SafeDestroy(s.gameObject);
        }
        spawnedBulletShooters.Clear();
        SafeStopCoroutine(ref shootCR, this);
    }

    private void StartFiringBullets(ShooterWrapper sw)
    {
        int totalBullets = settings.BulletsPerShot(Rage);

        for (int i = 0; i < totalBullets; i++)
        {
            float spawnLaneOffset = (i - i / 2) * terrSettings.TileDims.x;
            Vector3 spawnPosRel2Boss = new Vector3(spawnLaneOffset, settings.HeightOfBullet - transform.position.y, 0);
            Shooter instance = Shooter.InstantiateShooterObject(transform, spawnPosRel2Boss, settings.ShootSettings(Rage));
            spawnedBulletShooters.Add(instance);
        }

        Debug.Log("Boss 1 bullets fired!");
    }

    private int OnLaneChange(LaneChange laneChange)
    {
        //float deltaX = laneChange.Dir * terrSettings.TileDims.x;
        //PositionChange(transform, deltaX, 0, 0);
        Debug.Log("boss 1 recognized lane changed: " + laneChange.Dir);
        return 0;
    }

    private void SpawnSequence()
    {
        Vector3 finalSpqwnPos = transform.position;
        transform.position = new Vector3(finalSpqwnPos.x, settings.SpawnYPos, finalSpqwnPos.z);
        //TODO: Camera resitricted zoom to show it appeared!
        StartCoroutine(SpawnSequenceCR(finalSpqwnPos));
    }

    private IEnumerator SpawnSequenceCR(Vector3 finalSpawnPos)
    {
        float currTime = 0;
        while(currTime < settings.SpawnTime)
        {
            currTime += Time.deltaTime;
            float yPos = Mathf.Lerp(
                settings.SpawnYPos, finalSpawnPos.y, EasedPercent(currTime / settings.SpawnTime));

            transform.position = new Vector3(finalSpawnPos.x, yPos, finalSpawnPos.z);
            yield return null;
        }

        transform.position = finalSpawnPos;
        currState.ModifyValue(Boss1State.IDLE);
    }
}
