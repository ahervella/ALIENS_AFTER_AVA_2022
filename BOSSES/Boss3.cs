using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

public class Boss3 : AAlienBoss<Boss3State, SO_Boss3Settings>
{
    [SerializeField]
    private SO_Boss3PatterSettings patternSettings = null;

    [SerializeField]
    private GameObject bossNode = null;

    [SerializeField]
    private List<Boss3CannonDrone> cannonDrones = new List<Boss3CannonDrone>();
    
    [SerializeField]
    private float rawSpawnHeight = default;

    [SerializeField]
    private float rawEndDeathHeightPos = default;

    [SerializeField]
    private PSO_LaneChange boss3LaneChangePSO = null;

    [SerializeField]
    private BossLaneChangeManager cannonDronesLaneChangeRef = null;

    [SerializeField]
    private Transform cannonDronesContainer = null;

    [SerializeField]
    private Transform leftMostEmptyDroneRef = null;

    [SerializeField]
    private Transform rightMostEmptyDroneRef = null;

    private Vector3 cachedBossNodeFinalLocalPos;
    private List<Vector3> cachedDroneFinalLocalPos;


    private Coroutine idlePhaseCR = null;
    private Coroutine shootPhaseCR = null;
    private Coroutine moveBossLocalLaneCR = null;

    private Coroutine cannonDronesLaneChangeCR = null;

    private bool currBossRightOrLeftLocalLane;

    protected override void ExtraRemoveBoss()
    {
    }

    protected override void InitDeath()
    {
        SafeStopCoroutine(ref moveBossLocalLaneCR, this);
        SafeStopCoroutine(ref idlePhaseCR, this);
        SafeStopCoroutine(ref shootPhaseCR, this);

       foreach(Boss3CannonDrone drone in cannonDrones)
       {
            drone.InitDeath();
       }

        StartCoroutine(DeathFallCR());
    }

    private IEnumerator DeathFallCR()
    {
        yield return new WaitForSeconds(settings.DroneDeathFallRandDelayRange);

        Vector3 startPos = bossNode.transform.localPosition;
        
        float perc = 0;
        while (perc < 1)
        {
            perc += Time.deltaTime / settings.DeathFallTime;

            float yPos = Mathf.Lerp(startPos.y, rawEndDeathHeightPos, EasedPercent(perc));
            bossNode.transform.localPosition = new Vector3(startPos.x, yPos, startPos.z);

            yield return null;
        }

        AE_RemoveBoss();
    }

    protected override void InitRage()
    {
    }

    protected override void OnBossTakeNonLethalHit()
    {
        currBossRightOrLeftLocalLane = !currBossRightOrLeftLocalLane;
        MoveBossToLocalLane(currBossRightOrLeftLocalLane);
    }

    protected override void OnBossAwake()
    {
        boss3LaneChangePSO.RegisterForPropertyChanged(OnBoss3LaneChange);
        StartCoroutine(SpawnSequenceCR());
    }

    private void OnBoss3LaneChange(LaneChange _, LaneChange newLC)
    {
        SafeStartCoroutine(ref cannonDronesLaneChangeCR, CannonDronesLaneChangeCR(newLC), this);
    }

    private IEnumerator CannonDronesLaneChangeCR(LaneChange lc)
    {
        int centerDroneIndex = cannonDrones.Count / 2 + cannonDronesLaneChangeRef.CurrLaneDeviation;

        float startLocalXPos = cannonDronesContainer.localPosition.x;
        float finalLocalXPos;
        
        if (centerDroneIndex >= cannonDrones.Count)
        {
            finalLocalXPos = rightMostEmptyDroneRef.localPosition.x; 
        }
        else if (centerDroneIndex < 0)
        {
            finalLocalXPos = leftMostEmptyDroneRef.localPosition.x;
        }
        else
        {
            finalLocalXPos = cachedDroneFinalLocalPos[centerDroneIndex].x;
        }

        float perc = 0;
        while (perc < 1)
        {
            perc += Time.deltaTime / lc.Time;
            float localXPos = Mathf.Lerp(startLocalXPos, finalLocalXPos, EasedPercent(perc));
            cannonDronesContainer.localPosition = new Vector3(
                localXPos,
                cannonDronesContainer.localPosition.y,
                cannonDronesContainer.localPosition.z);
            yield return null;
        }
    }

    private IEnumerator SpawnSequenceCR()
    {
        yield return new WaitForSeconds(settings.SpawnDelay);

        for (int i = 0; i < cannonDrones.Count; i++)
        {
            StartCoroutine(MoveToSpawnPos(cannonDrones[i].gameObject, cachedDroneFinalLocalPos[i]));
            yield return new WaitForSeconds(settings.CannonSpawnTransitionDelay);
        }

        yield return MoveToSpawnPos(bossNode, cachedBossNodeFinalLocalPos);
        
        currZonePhase.ModifyValue(ZonePhaseEnum.BOSS);

        currState.RegisterForPropertyChanged(OnStateChange);
        yield return new WaitForSeconds(settings.FirstShootFromSpawnDelay);
        currState.ModifyValue(Boss3State.SHOOT);
    }

    private void OnStateChange(Boss3State _, Boss3State newState)
    {
        switch (newState)
        {
            case Boss3State.IDLE:
                SafeStartCoroutine(ref idlePhaseCR, IdlePhaseCR(), this);
                return;

            case Boss3State.SHOOT:
                SafeStartCoroutine(ref shootPhaseCR, ShootPhaseCR(), this);
                return;
        }
    }

    private IEnumerator MoveToSpawnPos(GameObject node, Vector3 finalLocalPos)
    {
        float perc = 0f;
        Vector3 startPos = node.transform.localPosition;

        while (perc < 1f)
        {
            perc += Time.deltaTime / settings.SpawnPosTransitionTime;
            node.transform.localPosition = Vector2.Lerp(
                startPos, finalLocalPos, EasedPercent(perc));
            yield return null;
        }
    }

    private IEnumerator IdlePhaseCR()
    {
        MoveBossToLocalLane(cannonDrones.Count / 2);
        cannonDronesLaneChangeRef.MoveToLane(0, interruptable: false);

        foreach(Boss3CannonDrone bcd in cannonDrones)
        {
            bcd.CleanUpShooter();
        }

        yield return new WaitForSeconds(settings.GetRandRangeIdlePhaseTime(Rage));

        currState.ModifyValue(Boss3State.SHOOT);
    }

    private IEnumerator ShootPhaseCR()
    {
        SO_Boss3PatternWrapper pw = patternSettings.GetRandPatternWrapper(currZonePhase.Value);
        
        currBossRightOrLeftLocalLane = Random.value > 0.5f;

        MoveBossToLocalLane(currBossRightOrLeftLocalLane);

        float timePassed = 0f;
        int i = 0;

        Vector2Int gridSize = pw.PatternSteps.GridSize;
        while (timePassed < pw.PatterDuration)
        {
            for (int k = 0; k < gridSize.x; k++)
            {
                if (!pw.PatternSteps.GetCell(k, i)) { continue; }
                cannonDrones[k].InstanceShooter(Rage);
            }
            yield return new WaitForSeconds(pw.NextStepDelay);
            timePassed += pw.NextStepDelay;
            i = (i + 1) % gridSize.y;
        }

        currState.ModifyValue(Boss3State.IDLE);
    }

    private void MoveBossToLocalLane(bool fullRightOrLeftSide)
    {
        MoveBossToLocalLane(fullRightOrLeftSide ? cannonDrones.Count - 1 : 0);
    }

    private void MoveBossToLocalLane(int cannonIndex)
    {
        SafeStartCoroutine(ref moveBossLocalLaneCR, MoveBossToCannonCR(cannonIndex), this);
    }

    private IEnumerator MoveBossToCannonCR(int cannonIndex)
    {
        moveBossLocalLaneCR = null;
        float localStartXPos = bossNode.transform.localPosition.x;
        float localEndXPos = cachedDroneFinalLocalPos[cannonIndex].x;

        float perc = 0;
        while (perc < 1)
        {
            perc += Time.deltaTime / settings.BossLocalLaneChangeTime;
            float xLocalPos = Mathf.Lerp(localStartXPos, localEndXPos, EasedPercent(perc));
            bossNode.transform.localPosition = new Vector3(
                xLocalPos,
                bossNode.transform.localPosition.y,
                bossNode.transform.localPosition.z
            );
            yield return null;
        }
    }

    protected override void SetStartingPosition()
    {
        float x = GetLaneXPosition(0, terrSettings);
        float z = settings.SpawnTileRowsAway * terrSettings.TileDims.y;
        transform.position = new Vector3(x, 0, z);

        cachedBossNodeFinalLocalPos = bossNode.transform.localPosition;

        bossNode.transform.localPosition = new Vector3(
            cachedBossNodeFinalLocalPos.x,
            rawSpawnHeight,
            cachedBossNodeFinalLocalPos.z);

        cachedDroneFinalLocalPos = new List<Vector3>();

        foreach (Boss3CannonDrone cannon in cannonDrones)
        {
            cachedDroneFinalLocalPos.Add(cannon.transform.localPosition);
            cannon.transform.localPosition = new Vector3(
                cannon.transform.localPosition.x,
                rawSpawnHeight,
                cannon.transform.localPosition.z);
        }
    }
}
