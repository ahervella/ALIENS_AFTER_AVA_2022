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

    public void AE_SpawnSplash(int spawnLocRefIndex)
    {
        GameObject instance = Instantiate(splashPrefab, terrNodes.Value.VerticalNode);

        Vector3 spawnPos = spawnLocRefIndex < orderedSpawnLocations.Count ?
            orderedSpawnLocations[spawnLocRefIndex].transform.position
            : transform.position;

        instance.transform.position = spawnPos;//new Vector3(spawnPos.x, 0, spawnPos.z);
    }
}
