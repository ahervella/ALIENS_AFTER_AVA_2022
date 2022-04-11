using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Boss1Settings", menuName = "ScriptableObjects/StaticData/SO_Boss1Settings")]
public class SO_Boss1Settings : SO_ABossSettings
{
    [SerializeField]
    private float prerageShootDelay = default;
    [SerializeField]
    private float rageShootDelay = default;

    public float ShootDelay(bool rage) => rage ? rageShootDelay : prerageShootDelay;

    [SerializeField]
    private int prerageShootRounds = default;
    [SerializeField]
    private int rageShootRounds = default;

    public int ShootRounds(bool rage) => rage ? rageShootRounds : prerageShootRounds;

    [SerializeField]
    private float spawnYPos = default;
    public float SpawnYPos => spawnYPos;

    [SerializeField]
    private float spawnTime = 3f;
    public float SpawnTime => spawnTime;
}

public enum Boss1State
{
    START = 0, SHOOT = 1, SHOOT_END = 7, IDLE = 2, LEFT = 3, RIGHT = 4, DEATH = 5, NONE = 6
}