using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "SO_TussleSettings", menuName = "ScriptableObjects/StaticData/SO_TussleSettings")]
public class SO_TussleSettings : ScriptableObject
{
    [SerializeField]
    private List<TussleVideoWrapper> wrappers = new List<TussleVideoWrapper>();
    public TussleVideoWrapper GetTussleVideoWrapper(TussleVideoType videoType)
    {
        foreach(TussleVideoWrapper tvw in wrappers)
        {
            if (tvw.VideoType == videoType)
            {
                return tvw;
            }
        }
        Debug.LogError($"No video found for tussle type {videoType}");
        return null;
    }
}

[Serializable]
public class TussleVideoWrapper
{
    [SerializeField]
    private TussleVideoType videoType;
    public TussleVideoType VideoType => videoType;

    [SerializeField]
    private VideoClip video = null;
    public VideoClip Video => video;

    [SerializeField]
    private bool loop;
    public bool Loop => loop;
}

public enum TussleVideoType
{
    ADV_START = 1,
    ADV_LOOP = 2,
    ADV_WIN = 3,
    ADV_LOOSE = 4,
    DIS_START = 5,
    DIS_LOOP = 6,
    DIS_WIN = 7,
    DIS_LOOSE = 8
}