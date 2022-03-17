using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static HelperUtil;

[CreateAssetMenu(fileName = "SO_TerrNodeFadeSettings", menuName = "ScriptableObjects/StaticData/SO_TerrNodeFadeSettings")]
public class SO_TerrNodeFadeSettings : ScriptableObject
{
    [SerializeField]
    private SO_TerrSettings terrSettings = null;

    [SerializeField]
    private SO_PlayerRunnerSettings playerSettings = null;


    [SerializeField]
    private float visibleYTilesFromPlayer = default;
    [NonSerialized]
    private float cachedVisibleZPos = default;
    public float CachedVisibleZPos => cachedVisibleZPos;

    [SerializeField]
    private float visibleXTilesFromPlayerSides = default;
    [NonSerialized]
    private float cachedVisibleXRightPos = default;
    public float CachedVisibleXRightPos => cachedVisibleXRightPos;
    [NonSerialized]
    private float cachedVisibleXLeftPos = default;
    public float CachedVisibleXLeftPos => cachedVisibleXLeftPos;



    [SerializeField]
    private float fadeYTileZDistance = default;
    [NonSerialized]
    private float cachedFadeZPos = default;
    public float CachedFadeZPos => cachedFadeZPos;

    [SerializeField]
    private float fadeXTileDistance = default;
    [NonSerialized]
    private float cachedFadeXRightPos;
    public float CachedFadeXRightPos => cachedFadeXRightPos;
    [NonSerialized]
    private float cachedFadeXLeftPos;
    public float CachedFadeXLeftPos => cachedFadeXLeftPos;

    [SerializeField]
    private float fadeYTileDisBehindPlayer = default;
    [NonSerialized]
    private float cachedFadeZPosBehindPlayer = default;
    public float CachedFadeZPosBehindPlayer => cachedFadeZPosBehindPlayer;


    [NonSerialized]
    private float cachedPlayerZPos = default;
    public float CachedPlayerZPos => cachedPlayerZPos;

    public void InitFadeSettings(Vector3 playerStartPos)
    {
        cachedPlayerZPos = playerStartPos.z;
        cachedVisibleZPos = cachedPlayerZPos + terrSettings.TileDims.y * visibleYTilesFromPlayer;

        cachedVisibleXRightPos = playerStartPos.x + terrSettings.TileDims.x * visibleXTilesFromPlayerSides;
        cachedVisibleXLeftPos = playerStartPos.x - terrSettings.TileDims.x * visibleXTilesFromPlayerSides;

        cachedFadeZPos = cachedVisibleZPos + terrSettings.TileDims.y * fadeYTileZDistance;

        cachedFadeXRightPos = cachedVisibleXRightPos + terrSettings.TileDims.x * fadeXTileDistance;
        cachedFadeXLeftPos = cachedVisibleXLeftPos - terrSettings.TileDims.x * fadeXTileDistance;

        cachedFadeZPosBehindPlayer = cachedPlayerZPos - terrSettings.TileDims.y * fadeYTileDisBehindPlayer;
    }
}
