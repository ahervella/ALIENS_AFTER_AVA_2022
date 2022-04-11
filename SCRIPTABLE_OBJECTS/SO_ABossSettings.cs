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
    private bool spawnAsChildOfTerr = false;
    public bool SpawnAsChildOfTerr => spawnAsChildOfTerr;

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
