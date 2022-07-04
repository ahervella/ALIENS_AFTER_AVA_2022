using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject splashPrefab = null;

    [SerializeField]
    private PSO_TerrainTreadmillNodes terrNodes = null;

    [SerializeField]
    private List<GameObject> orderedSpawnLocations = new List<GameObject>();

    [SerializeField]
    private IntPropertySO currZone = null;

    [SerializeField]
    private PSO_CurrentZonePhase currZonePhase = null;

    [SerializeField]
    private int zoneForSplash = 2;

    public void AE_SpawnSplash(int spawnLocRefIndex)
    {
        if (currZone.Value != zoneForSplash) { return; }

        //TODO: sync up with terrain change delegate to get to change to splashing soon as we get there

        GameObject instance = Instantiate(splashPrefab);
        terrNodes.Value.AttachTransform(instance.transform, horizOrVert: false);

        Vector3 spawnPos = spawnLocRefIndex < orderedSpawnLocations.Count ?
            orderedSpawnLocations[spawnLocRefIndex].transform.position
            : transform.position;

        instance.transform.position = spawnPos;
    }
}
