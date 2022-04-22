using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SO_SplashScreenSettings", menuName = "ScriptableObjects/StaticData/SO_SplashScreenSettings")]
public class SO_SplashScreenSettings : ScriptableObject
{
    [SerializeField]
    private List<SplashScreenWrapper> wrappers = new List<SplashScreenWrapper>();

    public List<SplashScreenWrapper> GetOrderedSplashScreenWrappers()
    {
        wrappers.Sort((a, b) => a.Order.CompareTo(b.Order));
        return wrappers;
    }
}

[Serializable]
public class SplashScreenWrapper
{
    [SerializeField]
    private int order = 0;
    public int Order => order;

    [SerializeField]
    private Sprite splashImage = null;
    public Sprite SplashImage => splashImage;

    [SerializeField]
    private float duration = 3f;
    public float Duration => duration;
}