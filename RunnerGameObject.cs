using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]
public class RunnerGameObject : MonoBehaviour
{
    public enum PLAYER_STATE { RUN, JUMP, LAND_G, DODGE_L, DODGE_R, ROLL, NONE, SPRINT}

    public BoxCollider hitBox;

    public AnimationClip[] animClips;

    protected Animator anim;
    protected AnimatorOverrideController animOC;

    protected string animIndex;


    public static float easingFunction(float theta)
    {
        return Mathf.Sin(theta - Mathf.PI / 2f) * 0.5f + 0.5f;
    }

    void OnEnable()
    {
        anim = gameObject.GetComponent<Animator>();
        animOC = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = animOC;

        animIndex = animOC.animationClips[0].name;

        hitBox = gameObject.GetComponent<BoxCollider>();
    }

}
