using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]
public class TerrObject : MonoBehaviour
{
    public enum OBJ_TYPE { STATIC, STATIC_HAZ, ENEMY, PLAYER};

    public OBJ_TYPE objType = OBJ_TYPE.STATIC;

    public AnimationClip[] animClips;

    protected Animator anim;
    protected AnimatorOverrideController animOC;

    string animIndex;

    public BoxCollider hitBox;
    public bool canFlip = true;

    public float appearanceLikelihood = 0.2f;

    public float elevationOffsetPerc = 0f;

    // Start is called before the first frame update
    void OnEnable()
    {
        anim = gameObject.GetComponent<Animator>();
        animOC = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = animOC;

        animIndex = animOC.animationClips[0].name;

        Debug.Log(animOC[animIndex]);

        hitBox = gameObject.GetComponent<BoxCollider>();
    }

    public void RandomizeSpriteType()
    {
        int index = Random.Range(0, animClips.Length);
        animOC[animIndex] = animClips[index];
    }
}
