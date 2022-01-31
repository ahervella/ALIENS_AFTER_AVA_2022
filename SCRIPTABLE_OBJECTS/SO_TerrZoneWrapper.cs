using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_TerrZoneWrapper", menuName = "ScriptableObjects/StaticData/SO_TerrZoneWrapper")]
public class SO_TerrZoneWrapper : ScriptableObject
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
        this.zone = zone;
        this.startSpeed = startSpeed;
        this.speedIncPerMin = speedIncPerMin;
        this.addonSpawnLikelihood = addonSpawnLikelihood;
        this.addonSpawnIncPerMin = addonSpawnIncPerMin;
        this.foleySpawnLikelihood = foleySpawnLikelihood;
        this.tileDistance2Boss = tileDistance2Boss;
    }

    [SerializeField]
    private int zone;
    public int Zone => zone;

    [SerializeField]
    private float startSpeed;
    public float StartSpeed => startSpeed;

    [SerializeField]
    private float speedIncPerMin;
    public float SpeedIncPerMin => speedIncPerMin;

    [SerializeField]
    private float addonSpawnLikelihood;
    public float AddonSpawnLikelihood => addonSpawnLikelihood;

    [SerializeField]
    private float addonSpawnIncPerMin;
    public float AddonSpawnIncPerMin => addonSpawnIncPerMin;

    [SerializeField]
    private float foleySpawnLikelihood;
    public float FoleySpawnLikelihood => foleySpawnLikelihood;

    [SerializeField]
    private float tileDistance2Boss;
    public float TileDistance2Boss => tileDistance2Boss;

    //public List<TerrAddonWeightWrapper> TerrAddons;

    //public List<TerrAddonWeightWrapper> TerrFoleyAddons;
}
