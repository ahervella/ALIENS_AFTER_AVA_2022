﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]
public class RunnerGameObject : MonoBehaviour
{
    const float GAME_FPS = 60f;

    public enum PLAYER_STATE { RUN, JUMP, LAND_G, DODGE_L, DODGE_R, ROLL, NONE, SPRINT, DEATH1, HURT_F, HURT_T, HURT_L, THROW_R, FIRE, THROW_G, TAKEDOWN1, TAKEDOWN2, TAKEDOWN3}

    public BoxCollider hitBox;

    public AnimationClip[] animClips;

    protected Animator anim;
    protected AnimatorOverrideController animOC;

    protected string animIndex;


    public static float EasingFunction(float theta)
    {
        float result = Mathf.Sin(theta - Mathf.PI / 2f) * 0.5f + 0.5f;
        return result < 0.0001 ? 0 : result;
    }

    public static float GetGameFPS() { return GAME_FPS; }

    protected void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        animOC = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = animOC;

        animIndex = animOC.animationClips[0].name;

        hitBox = gameObject.GetComponent<BoxCollider>();
    }

}
