using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_TerrZoneWrapper : MonoBehaviour
{
    public SO_TerrZoneWrapper(
        int zone,
        float startSpeed,
        float speedIncPerMin,
        float addonSpawnLikelihood,
        float addonSpawnIncPerMin,
        float foleySpawnLikelihood,
        float tileDistance2Boss
        //List<TerrAddonWeightWrapper> terrAddons
        //List<TerrAddonWeightWrapper> terrFoleyAddons
        )
    {
        Zone = zone;
        StartSpeed = startSpeed;
        SpeedIncPerMin = speedIncPerMin;
        AddonSpawnLikelihood = addonSpawnLikelihood;
        AddonSpawnIncPerMin = addonSpawnIncPerMin;
        FoleySpawnLikelihood = foleySpawnLikelihood;
        TileDistance2Boss = tileDistance2Boss;
    } 

    public int Zone { get; private set; }

    public float StartSpeed { get; private set; }

    public float SpeedIncPerMin { get; private set; }

    public float AddonSpawnLikelihood { get; private set; }

    public float AddonSpawnIncPerMin { get; private set; }

    public float FoleySpawnLikelihood { get; private set; }

    public float TileDistance2Boss { get; private set; }

    //public List<TerrAddonWeightWrapper> TerrAddons;

    //public List<TerrAddonWeightWrapper> TerrFoleyAddons;
}
