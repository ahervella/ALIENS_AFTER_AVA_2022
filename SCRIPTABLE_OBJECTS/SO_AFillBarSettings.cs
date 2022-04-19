using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SO_AFillBarSettings : ScriptableObject
{
    [SerializeField]
    private bool fadeIn = true;
    public bool FadeIn => fadeIn;

    [SerializeField]
    private float fadeInDelay = 1f;
    public float FadeInDelay => fadeInDelay;

    [SerializeField]
    private float fadeInTime = 1f;
    public float FadeInTime => fadeInTime;

    [SerializeField]
    private bool spawnFromTop = true;
    public bool SpawnFromTop => spawnFromTop;

    [SerializeField]
    private float spawnFromTopTime = 3f;
    public float SpawnFromTopTime => spawnFromTopTime;

    [SerializeField]
    private bool animateBarFillOnSpawn = true;
    public bool AnimateBarFillOnSpawn => animateBarFillOnSpawn;

    [SerializeField]
    private float barFillOnSpawnDelay = 1f;
    public float BarFillOnSpawnDelay => barFillOnSpawnDelay;

    [SerializeField]
    private float barFillOnSpawnTime = 2f;
    public float BarFillOnSpawnTime => barFillOnSpawnTime;

    [SerializeField]
    private int maxQuant = default;
    public int MaxQuant => maxQuant;

    [SerializeField]
    private int startingQuant = default;
    public int StartingQuant => startingQuant;
}
