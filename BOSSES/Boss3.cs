using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

[RequireComponent(typeof(BossLaneChangeManager))]
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

    private Vector3 cachedBossNodeFinalLocalPos;
    private List<Vector3> cachedDroneFinalLocalPos;

    private BossLaneChangeManager laneChangeManager = null;

    private Coroutine moveBossLocalLaneCR = null;

    private bool currBossRightOrLeftLocalLane;

    protected override void ExtraRemoveBoss()
    {
    }

    protected override void InitDeath()
    {
        
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
        laneChangeManager = GetComponent<BossLaneChangeManager>();
        StartCoroutine(SpawnSequenceCR());
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

        StartCoroutine(IdlePhaseCR());
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
        MoveBossToLocalLane(0);
        foreach(Boss3CannonDrone bcd in cannonDrones)
        {
            bcd.CleanUpShooter();
        }

        laneChangeManager.MoveToLane(0);

        yield return new WaitForSeconds(settings.GetRandRangeIdlePhaseTime(Rage));
        
        //put instead in action change
        StartCoroutine(ShootPhaseCR());
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
                cannonDrones[k].InstanceShooter(HitBox(), Rage);
            }
            yield return new WaitForSeconds(pw.NextStepDelay);
            timePassed += pw.NextStepDelay;
            i = (i + 1) % gridSize.y;
        }

        //put instead in action change
        StartCoroutine(IdlePhaseCR());
    }

    private void MoveBossToLocalLane(bool fullRightOrLeftSide)
    {
        MoveBossToLocalLane((fullRightOrLeftSide ? cannonDrones.Count - 1 : 0) - cannonDrones.Count / 2);
    }

    private void MoveBossToLocalLane(int localLaneIndex)
    {
        SafeStartCoroutine(ref moveBossLocalLaneCR, MoveBossToLocalLaneCR(localLaneIndex), this);
    }

    private IEnumerator MoveBossToLocalLaneCR(int localLaneIndex)
    {
        moveBossLocalLaneCR = null;
        float localStartXPos = bossNode.transform.localPosition.x;
        float localEndXPos = GetLaneXPosition(
            localLaneIndex + laneChangeManager.CurrLaneDeviation, terrSettings) - transform.position.x;

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
        moveBossLocalLaneCR = null;
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
