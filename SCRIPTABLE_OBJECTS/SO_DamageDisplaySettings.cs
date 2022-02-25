using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SO_DamageDisplaySettings", menuName = "ScriptableObjects/StaticData/SO_DamageDisplaySettings")]
public class SO_DamageDisplaySettings : ScriptableObject
{
    [SerializeField]
    private Material camTintMat = null;
    public Material CamTintMat => camTintMat;

    [SerializeField]
    private float tintTweenTime = 1f;
    public float TintTweenTime => tintTweenTime;

    [SerializeField]
    private Color hurtTintColor = default;
    public Color HurtTintColor => hurtTintColor;

    [SerializeField]
    private float damageImpactAlpha = 1f;
    public float DamageImpactAlpha => damageImpactAlpha;

    [SerializeField]
    private float damageImpactTweenTime = 1f;
    public float DamageImpactTweenTime => damageImpactTweenTime;

    [SerializeField]
    private List<DamageWrapper> damageWrappers = new List<DamageWrapper>();

    [SerializeField]
    private DamageWrapper defaultDamageWrapper = new DamageWrapper();

    public DamageWrapper GetDamageWrapper(int lifeCount)
    {
        foreach (DamageWrapper dw in damageWrappers)
        {
            if (dw.LifeAmount == lifeCount)
            {
                return dw;
            }
        }

        return defaultDamageWrapper;
    }

    [SerializeField]
    private List<Sprite> damageImpactSprites = new List<Sprite>();

    public Sprite GetRandomImpactSprite()
    {
        int randIndex = UnityEngine.Random.Range(0, damageImpactSprites.Count);
        return damageImpactSprites[randIndex];
    }

}

[Serializable]
public class DamageWrapper
{
    [SerializeField]
    private int lifeAmount = default;
    public int LifeAmount => lifeAmount;

    [SerializeField]
    private float tintPercent = default;
    public float TintPercent => tintPercent;

    [SerializeField]
    private float damageAlphaPercentMax = default;
    public float DamageAlphaPercentMax => damageAlphaPercentMax;

    [SerializeField]
    private float damageAlphaPercentMin = default;
    public float DamageAlphaPercentMin => damageAlphaPercentMin;

    [SerializeField]
    private float damageAlphaPulseTime = default;
    public float DamageAlphaPulseTime => damageAlphaPulseTime;
}
