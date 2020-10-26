using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RunnerPlayer : RunnerGameObject
{
    const int DODGE_FRAME_DELAY = 1;
    const float SPRINT_TIME = 1.5f;
    const float SPRINT_COOL_DOWN_TIME = 3f;
    const float SPRINT_SPEED_PERCENT_BOOST = 0.3f;
    const float minCollideDist = 1f;

    //List<TerrObject> checkClipObjs = new List<TerrObject>();

    //const int JUMP_FRAME_DELAY = 2;

    public int startingLives;
    int lives;

    public float lifeRecoverTime;
    float lifeRecoverTotalTime = 0f;

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


    public RunnerThrowable[] throwables;
    Dictionary<RunnerThrowable.THROW_TYPE, RunnerThrowable> throwablesDict = new Dictionary<RunnerThrowable.THROW_TYPE, RunnerThrowable>();

    PLAYER_STATE currState = PLAYER_STATE.RUN;

    public static event System.Action<PLAYER_STATE> onAnimationStarted = delegate { };
    public static event System.Action<PLAYER_STATE> onAnimationEnded = delegate { };
    public static event System.Action<bool, float, float> changeTreamillSpeed = delegate { };
    public static event System.Action<float, int, bool> changeCamRedTint = delegate { };
    //public static event System.Action<TerrObject> removeTerrObj
 
    // Update is called once per frame
    private void Start()
    {

        lives = startingLives;

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


        foreach (RunnerThrowable throwable in throwables)
        {
            throwablesDict.Add(throwable.throwType, throwable);
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

        //if (terrObj2Far2Collide()) { return; }

        if (terrObj.objType == TerrObject.OBJ_TYPE.ROCK)
        {
            if (terrObj.actionNeeded == currState)
            {
                grabbedRock(terrObj);
            }
            return;
        }

        if (terrObj.objType != TerrObject.OBJ_TYPE.STATIC && terrObj.actionNeeded != currState)
        {
            
            looseLife(terrObj);
            Debug.Log("IM HIT!!!!");
        }

        
    }

    void grabbedRock(TerrObject rockObj)
    {
        if (hasRock) { return; }

        Debug.Log("GOT ROCK!");

        hasRock = true;

        //make rock "disappear"
        rockObj.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
    }

    //Attempt at basically raycasting for fast moving objects
    /*
    bool terrObj2Far2Collide(TerrObject terrObj)
    {

        if (terrObj.transform.position.z - transform.position.z <= minCollideDist)
        {
            return false;
        }
        else
        {
            checkClipObjs.Add(terrObj);
            return true;
        }
    }
    */


    private void looseLife(TerrObject hazObject)
    {
        if (currState == PLAYER_STATE.DEATH1
            || currState == PLAYER_STATE.HURT_F
            || currState == PLAYER_STATE.HURT_L
            || currState == PLAYER_STATE.HURT_T)
        {
            return;
        }



        lives--;
        lifeRecoverTotalTime = 0f;

        if (gameIsOver())
        {
            changeCamRedTint(2f, 1, true);
        }
        else
        {
            switch (lives)
            {
                case 2:
                    changeCamRedTint(2f, 3, false);
                    break;

                case 1:
                    changeCamRedTint(1.5f, 5, false);
                    break;

                case 0:
                    changeCamRedTint(1f, 10, false);
                    break;
            }
        }

        PLAYER_STATE state = PLAYER_STATE.NONE;

        switch (hazObject.actionNeeded)
        {
            case PLAYER_STATE.NONE:
                state = PLAYER_STATE.HURT_F;
                break;

            case PLAYER_STATE.ROLL:
                state = PLAYER_STATE.HURT_T;
                break;

            case PLAYER_STATE.JUMP:
                state = PLAYER_STATE.HURT_L;
                break;
        }

        defaultInitAction(state);

    }

    
    private void FixedUpdate()
    {
        if (lives >= startingLives) { return; }

        lifeRecoverTotalTime += Time.deltaTime;

        if (lifeRecoverTotalTime >= lifeRecoverTime)
        {
            lives++;
            lifeRecoverTotalTime = 0f;
        }
    }
    

    //controled by event in animation



    void treadmillOff(int changeTimeInFrames) { treadmillSpeedChange(false, changeTimeInFrames); }

    void treadmillOn(int changeTimeInFrames) { treadmillSpeedChange(true, changeTimeInFrames); }

    void treadmillSpeedChange(bool treadmillOn, int changeTimeInFrames, float speedMultiplyer = 1f)//, int changeTimeInFrames)
    {
        float changeTime = getTimeFromFrames(animDict[currState.ToString()], changeTimeInFrames);
        //for smoother animation treadmill speed change
        changeTime = gameIsOver() && treadmillOn && currState != PLAYER_STATE.DEATH1? changeTime * 4 : changeTime;
        changeTreamillSpeed(treadmillOn, changeTime, speedMultiplyer);
    }

    bool gameIsOver()
    {
        return lives < 0;
    }

    void playAnimSound(SoundWrapper sw)
    {
        RunnerSounds.Current.playSound(sw, this.gameObject);
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

        if (gameIsOver() && animState != PLAYER_STATE.DEATH1)
        {
            initDeath();
            onAnimationEnded(animState);
            return;
        }

        switch (animState)
        {
            case PLAYER_STATE.DEATH1:
                    return;

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


    public void initDeath()
    {
        switchAnimState(PLAYER_STATE.DEATH1);
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
        treadmillSpeedChange(true, 6, 1f + SPRINT_SPEED_PERCENT_BOOST);
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
        treadmillSpeedChange(true, 6);
        yield return new WaitForSecondsRealtime(SPRINT_COOL_DOWN_TIME);
        canSprint = true;
        sprintCoolDownCoroutine = null;

    }



    public void throwRock()
    {
        if (!hasRock) { return; }
        defaultInitAction(PLAYER_STATE.THROW_R);
        hasRock = false;
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


        string stateString = state.ToString();


        int currState = anim.GetCurrentAnimatorStateInfo(0).fullPathHash;
        float startNormTime = getNormTimeFromFrame(animDict[stateString], anim, startFrame);

        


        string LAstring = "LA_" + stateString;
        LAstring = gunBullets > 0 && state != PLAYER_STATE.THROW_G? LAstring + "_GUN" : LAstring;
        int currStateLA = animLA.GetCurrentAnimatorStateInfo(0).fullPathHash;
        float startNormTimeLA = getNormTimeFromFrame(animLADict[LAstring], animLA, startFrame);
        

        string RAstring = "RA_" + stateString;
        RAstring = hasRock && state != PLAYER_STATE.THROW_R? RAstring + "_ROCK" : RAstring;
        
        int currStateRA = animRA.GetCurrentAnimatorStateInfo(0).fullPathHash;
        float startNormTimeRA = getNormTimeFromFrame(animRADict[RAstring], animRA, startFrame);

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


        animOC[animIndex] = animDict[stateString];
        animLAOC[animLAIndex] = animLADict[LAstring];
        animRAOC[animRAIndex] = animRADict[RAstring];

    }



    public void generateThrowable(RunnerThrowable.THROW_TYPE throwType)
    {
        throwablesDict[throwType].Instantiate(transform);
    }

    int getCurrFrame(AnimationClip animClip, Animator animator)
    {
        float normTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        float animLength = animClip.length;
        float animFrameRate = animClip.frameRate;
        return (int)(normTime * animLength * animFrameRate);
    }

    float getTimeFromFrames(AnimationClip animClip, int frameCount)
    {
        return animClip.frameRate / RunnerGameObject.getGameFPS() * frameCount;
    }

    float getNormTimeFromFrame(AnimationClip animClip, Animator animator, int frame)
    {
        float normTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        float animLength = animClip.length;
        float animFrameRate = animClip.frameRate;
        return (float)frame / (animLength * animFrameRate);
    }
}
