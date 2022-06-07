using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HelperUtil;

public class Boss3 : AAlienBoss<Boss3State, SO_Boss3Settings>
{
    [SerializeField]
    private GameObject bossNode = null;

    [SerializeField]
    private List<GameObject> cannonDrones = new List<GameObject>();

    private Vector3 cachedBossNodeFinalLocalPos;
    private List<Vector3> cachedDroneFinalLocalPos;

    [SerializeField]
    private float rawSpawnHeight = default;

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
    }

    private IEnumerator SpawnSequenceCR()
    {
        yield return new WaitForSeconds(settings.SpawnDelay);

        for (int i = 0; i < cannonDrones.Count; i++)
        {
            yield return MoveToSpawnPos(cannonDrones[i], cachedDroneFinalLocalPos[i]);
            yield return new WaitForSeconds(settings.CannonSpawnTransitionDelay);
        }

        yield return MoveToSpawnPos(bossNode, cachedBossNodeFinalLocalPos);
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

    protected override void SetStartingPosition()
    {
        float x = GetLaneXPosition(0, terrSettings);
        float z = settings.SpawnTileRowsAway * terrSettings.TileDims.y;
        transform.position = new Vector3(x, 0, z);

        bossNode.transform.localPosition = new Vector3(0, rawSpawnHeight, 0);

        cachedDroneFinalLocalPos = new List<Vector3>();

        foreach (GameObject cannon in cannonDrones)
        {
            cachedDroneFinalLocalPos.Add(cannon.transform.localPosition);
            cannon.transform.localPosition = new Vector3(
                cannon.transform.position.x, rawSpawnHeight, 0);
        }
    }
}
