using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

[RequireComponent(typeof(BossLaneChangeManager))]
public class Boss3 : AAlienBoss<Boss3State, SO_Boss3Settings>
{
    [SerializeField]
    private GameObject bossNode = null;

    [SerializeField]
    private List<Boss3CannonDrone> cannonDrones = new List<Boss3CannonDrone>();
    
    [SerializeField]
    private float rawSpawnHeight = default;

    private Vector3 cachedBossNodeFinalLocalPos;
    private List<Vector3> cachedDroneFinalLocalPos;

    private BossLaneChangeManager laneChangeManager = null;


    protected override void ExtraRemoveBoss()
    {
    }

    protected override void InitDeath()
    {
        
    }

    protected override void InitRage()
    {
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
        foreach(Boss3CannonDrone bcd in cannonDrones)
        {
            bcd.CleanUpShooter();
        }
        yield return new WaitForSeconds(settings.GetRandRangeIdlePhaseTime(Rage));
        
        //put instead in action change
        StartCoroutine(ShootPhaseCR());
    }

    private IEnumerator ShootPhaseCR()
    {
        int CurrLaneDeviation() => laneChangeManager.CurrLaneDeviation;

        foreach(Boss3CannonDrone bcd in cannonDrones)
        {
            bcd.InstanceShooter(
                CurrLaneDeviation,
                HitBox(),
                Rage);
        }

        yield return new WaitForSeconds(settings.ShootPhaseTime(Rage));

        //put instead in action change
        StartCoroutine(IdlePhaseCR());
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
