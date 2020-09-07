using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RunnerPlayer : RunnerGameObject
{
    const int DODGE_FRAME_DELAY = 1;
    const float SPRINT_TIME = 1.5f;
    const float SPRINT_COOL_DOWN_TIME = 3f;
    //const int JUMP_FRAME_DELAY = 2;

    public int lives;

    bool canChange = true;
    bool canSprint = true;
    Coroutine sprintCoroutine;
    Coroutine sprintCoolDownCoroutine;
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

    public static event System.Action<PLAYER_STATE> onAnimationStarted = delegate { };
    public static event System.Action<PLAYER_STATE> onAnimationEnded = delegate { };

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
        PLAYER_STATE animState = PLAYER_STATE.NONE;

        foreach (PLAYER_STATE st in System.Enum.GetValues(typeof(PLAYER_STATE)))
        {
            if (animClip.name == st.ToString())
            {
                animState = st;
                break;
            }
        }


        switch (animState)
        {
            case PLAYER_STATE.JUMP:
                switchAnimState(PLAYER_STATE.LAND_G);
                //controls feel better this way
                canChange = true;
                if (!canSprint && sprintCoolDownCoroutine == null)
                {
                    sprintCoolDownCoroutine = StartCoroutine(sprintCoolDown());
                }
                break;


            case PLAYER_STATE.LAND_G:
                canChange = true;
                switchAnimState(PLAYER_STATE.RUN);
                break;


            case PLAYER_STATE.DODGE_R:
            case PLAYER_STATE.ROLL:
                switchAnimState(PLAYER_STATE.RUN, 7);
                canChange = true;
                if (!canSprint && sprintCoolDownCoroutine == null)
                {
                    sprintCoolDownCoroutine = StartCoroutine(sprintCoolDown());
                }
                break;


            default:
                switchAnimState(PLAYER_STATE.RUN);
                canChange = true;
                if (!canSprint && sprintCoolDownCoroutine == null)
                {
                    sprintCoolDownCoroutine = StartCoroutine(sprintCoolDown());
                }
                break;
        }


        onAnimationEnded(animState);

    }




    public float dodge(bool dodgeRight)
    {
        float delayFromDodge = 0;

        if (dodgeRight)
        {
            defaultInitAction(PLAYER_STATE.DODGE_R);
            delayFromDodge = getNormTimeFromFrame(animDict[PLAYER_STATE.DODGE_R.ToString()], anim, DODGE_FRAME_DELAY);
        }
        else
        {
            defaultInitAction(PLAYER_STATE.DODGE_L);
            delayFromDodge = getNormTimeFromFrame(animDict[PLAYER_STATE.DODGE_L.ToString()], anim, DODGE_FRAME_DELAY);
        }

        return delayFromDodge;

    }

    public void jump()
    {
        defaultInitAction(PLAYER_STATE.JUMP);

    }

    public void roll()
    {
        defaultInitAction(PLAYER_STATE.ROLL);
    }



    public void sprint()
    {
        if (!canSprint || !canChange) { return; }
        switchAnimState(PLAYER_STATE.SPRINT);
        sprintCoroutine = StartCoroutine(sprintInitTimer());
    }

    IEnumerator sprintInitTimer()
    {
        canSprint = false;
        yield return new WaitForSecondsRealtime(SPRINT_TIME);
        switchAnimState(PLAYER_STATE.RUN);
        sprintCoroutine = null;
        sprintCoolDownCoroutine = StartCoroutine(sprintCoolDown());
    }

    IEnumerator sprintCoolDown()
    {
        onAnimationEnded(PLAYER_STATE.SPRINT);
        yield return new WaitForSecondsRealtime(SPRINT_COOL_DOWN_TIME);
        canSprint = true;
        sprintCoolDownCoroutine = null;
        
    }




    void defaultInitAction(PLAYER_STATE state)
    {
        switchAnimState(state);
        canChange = false;
        if (sprintCoroutine != null) { StopCoroutine(sprintCoroutine); sprintCoroutine = null; }
    }

    void switchAnimState(PLAYER_STATE state, int startFrame = 0)
    {
        currState = state;
        StartCoroutine(switchAnim(state, startFrame));
    }

    IEnumerator switchAnim(PLAYER_STATE state, int startFrame = 0)
    {
        onAnimationStarted(state);


        int currState = anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
        float startNormTime = getNormTimeFromFrame(animDict[state.ToString()], anim, startFrame);
        
        

        string LAstring = "LA_" + state.ToString();
        LAstring = gunBullets > 0 ? LAstring + "_GUN" : LAstring;
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
