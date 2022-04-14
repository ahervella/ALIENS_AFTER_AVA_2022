using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static HelperUtil;

public abstract class SO_ABossSettings : ScriptableObject
{
    [SerializeField]
    private int startingHealth = default;
    public int StartingHealth => startingHealth;

    [SerializeField]
    private float rageHealthThreshold = default;
    public float RageHealthThreshold => rageHealthThreshold;

    [SerializeField]
    private int hitBoxTileWidth = 1;
    public int HitBoxTileWidth => hitBoxTileWidth;

    [SerializeField]
    private int spawnTileRowsAway = 5;
    public int SpawnTileRowsAway => spawnTileRowsAway;

    [SerializeField]
    private BossSpawnEnum spawnType = BossSpawnEnum.INDEPENDENT;
    public BossSpawnEnum SpawnType => spawnType;

    [Serializable]
    protected class RageValue<T>
    {
        [SerializeField]
        private T prerageVal = default;

        [SerializeField]
        private T rageVal = default;

        public T GetVal(bool rage) => rage ? rageVal : prerageVal;
    }
    /*
    [SerializeField]
    private List<BossAnimationWrapper<BOSS_STATE>> animWrappers = new List<BossAnimationWrapper<BOSS_STATE>>();

    public AnimationClip GetBossAnim(BOSS_STATE state)
    {
        BossAnimationWrapper<BOSS_STATE> wrapper = GetWrapperFromFunc(
            animWrappers, baw => baw.State, state, LogEnum.ERROR, null);

        return wrapper.Anim;
    }

    [Serializable]
    private class BossAnimationWrapper<T>
    {
        [SerializeField]
        private T state = default;
        public T State => state;

        [SerializeField]
        private AnimationClip anim = null;
        public AnimationClip Anim => anim;
    }
    */
}

public enum BossSpawnEnum { INDEPENDENT = 0, TERR_HORIZ = 1, TERR_VERT = 2 }
