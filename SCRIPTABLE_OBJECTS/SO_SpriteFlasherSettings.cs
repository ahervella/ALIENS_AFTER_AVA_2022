using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_SpriteFlasherSettings", menuName = "ScriptableObjects/StaticData/SO_SpriteFlasherSettings")]
public class SO_SpriteFlasherSettings : ScriptableObject
{
    [SerializeField]
    private float flashLoopTime = 1f;
    public float FlashLoopTime => flashLoopTime;

    [SerializeField]
    private Color flashColor = default;
    public Color FlashColor => flashColor;
}
