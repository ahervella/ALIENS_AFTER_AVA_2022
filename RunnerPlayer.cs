using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RunnerPlayer : RunnerGameObject
{
    const int DODGE_FRAME_DELAY = 1;
    //const int JUMP_FRAME_DELAY = 2;

    public int lives;

    bool canChange = true;
    public bool canChangeState() { return canChange; }

    bool hasRock = false;
    int gunBullets = 0;

    Dictionary<string, AnimationClip> animDict = new Dictionary<string, AnimationClip>();

    public GameObject LA;
    public GameObject RA;

    protected Animator animLA;
    protected AnimatorOverrideController animLAOC;
    protected string animLAIndex;
    public AnimationClip[] animLAClips;
    Dictionary<string, AnimationClip> animLADict = new Dictionary<string, AnimationClip>();

    protected Animator animRA;
    protected AnimatorOverrideController animRAOC;
    protected string animRAIndex;
    public AnimationClip[] animRAClips;
    Dictionary<string, AnimationClip> animRADict = new Dictionary<string, AnimationClip>();


    PLAYER_STATE currState = PLAYER_STATE.RUN;


    // Update is called once per frame
    private void Start()
    {
        foreach (AnimationClip animClip in animClips)
        {
            animDict.Add(animClip.name, animClip);
        }

        foreach (AnimationClip animLAClip in animLAClips)
        {
            animLADict.Add(animLAClip.name, animLAClip);
        }

        foreach (AnimationClip animRAClip in animRAClips)
        {
            animRADict.Add(animRAClip.name, animRAClip);
        }


        animLA = LA.GetComponent<Animator>();
        animRA = RA.GetComponent<Animator>();

        animLAOC = new AnimatorOverrideController(animLA.runtimeAnimatorController);
        animLA.runtimeAnimatorController = animLAOC;
        animLAIndex = animLAOC.animationClips[0].name;

        animRAOC = new AnimatorOverrideController(animRA.runtimeAnimatorController);
        animRA.runtimeAnimatorController = animRAOC;
        animRAIndex = animRAOC.animationClips[0].name;

        switchAnimState(PLAYER_STATE.RUN);
    }


    private void OnTriggerEnter(Collider coll)
    {
        TerrObject terrObj = coll.gameObject.GetComponent<TerrObject>();
        if (terrObj == null) { return;  }

        if (terrObj.objType != TerrObject.OBJ_TYPE.ENEMY && terrObj.objType == TerrObject.OBJ_TYPE.STATIC_HAZ) { return; }


        if (terrObj.actionNeeded != currState)
        {
            Debug.Log("IM HIT!!!!");
        }

        
    }

    void onAnimEnd(AnimationClip animClip)
    {

        if (animClip.name == PLAYER_STATE.JUMP.ToString())
        {
            switchAnimState(PLAYER_STATE.LAND_G);
        }

        else if (animClip.name == PLAYER_STATE.DODGE_R.ToString()
            || animClip.name == PLAYER_STATE.ROLL.ToString())
        {
            switchAnimState(PLAYER_STATE.RUN, 7);
            canChange = true;
        }

        else
        {
            switchAnimState(PLAYER_STATE.RUN);
            canChange = true;
        }
    }

    public float dodge(bool dodgeRight)
    {
        float delayFromDodge = 0;

        if (dodgeRight)
        {
            switchAnimState(PLAYER_STATE.DODGE_R);
            delayFromDodge = getNormTimeFromFrame(animDict[PLAYER_STATE.DODGE_R.ToString()], anim, DODGE_FRAME_DELAY);
        }
        else
        {
            switchAnimState(PLAYER_STATE.DODGE_L);
            delayFromDodge = getNormTimeFromFrame(animDict[PLAYER_STATE.DODGE_L.ToString()], anim, DODGE_FRAME_DELAY);
        }

        canChange = false;

        return delayFromDodge;

    }

    public void jump()
    {
        switchAnimState(PLAYER_STATE.JUMP);
        canChange = false;

        //return getNormTimeFromFrame(animDict[PLAYER_STATE.JUMP.ToString()], anim, JUMP_FRAME_DELAY);
    }

    public void roll()
    {
        switchAnimState(PLAYER_STATE.ROLL);
        canChange = false;
    }

    void switchAnimState(PLAYER_STATE state, int startFrame = 0)
    {

        currState = state;
        StartCoroutine(switchAnim(state, startFrame));
    }

    IEnumerator switchAnim(PLAYER_STATE state, int startFrame = 0)
    {
        //animOC[animIndex] = animDict[state.ToString()];
        int currState = anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
        float startNormTime = getNormTimeFromFrame(animDict[state.ToString()], anim, startFrame);
        
        

        string LAstring = "LA_" + state.ToString();
        LAstring = gunBullets > 0 ? LAstring + "_GUN" : LAstring;
        //animLAOC[animLAIndex] = animLADict[LAstring];
        int currStateLA = animLA.GetCurrentAnimatorStateInfo(0).fullPathHash;
        float startNormTimeLA = getNormTimeFromFrame(animLADict[LAstring.ToString()], animLA, startFrame);
        

        string RAstring = "RA_" + state.ToString();
        RAstring = hasRock ? RAstring + "_ROCK" : RAstring;
        
        int currStateRA = animRA.GetCurrentAnimatorStateInfo(0).fullPathHash;
        float startNormTimeRA = getNormTimeFromFrame(animRADict[RAstring.ToString()], animRA, startFrame);
        /*
        anim.PlayInFixedTime(currState, 0, startNormTime);
        animLA.PlayInFixedTime(currStateLA, 0, startNormTimeLA);
        animRA.PlayInFixedTime(currStateRA, 0, startNormTimeRA);
        */

        anim.Play(currState, 0, startNormTime);
        animLA.Play(currStateLA, 0, startNormTimeLA);
        animRA.Play(currStateRA, 0, startNormTimeRA);


        //fuccccck this was such a bitch, need this specifically end of frame
        //or else playing from 0 wont happen at the same time as changing anim
        yield return new WaitForEndOfFrame();


        animOC[animIndex] = animDict[state.ToString()];
        animLAOC[animLAIndex] = animLADict[LAstring];
        animRAOC[animRAIndex] = animRADict[RAstring];

    }

    int getCurrFrame(AnimationClip animClip, Animator animator)
    {
        float normTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        float animLength = animClip.length;
        float animFrameRate = animClip.frameRate;
        return (int)(normTime * animLength * animFrameRate);
    }

    float getNormTimeFromFrame(AnimationClip animClip, Animator animator, int frame)
    {
        float normTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        float animLength = animClip.length;
        float animFrameRate = animClip.frameRate;
        return (float)frame / (animLength * animFrameRate);
    }
}
