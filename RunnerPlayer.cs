using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public static class PSAAW_ExtensionMethods
{
    
    public static bool Contains(this List<RunnerPlayer.PlayerStateAAudioWrapperKVP> kvpList, RunnerGameObject.PLAYER_STATE keyIndex)
    {
        foreach(RunnerPlayer.PlayerStateAAudioWrapperKVP kvp in kvpList)
        {
            if (kvp.key == keyIndex)
            {
                return true;
            }
        }
        return false;
    }

    public static AAudioWrapper GetAAW(this List<RunnerPlayer.PlayerStateAAudioWrapperKVP> kvpList, RunnerGameObject.PLAYER_STATE keyIndex)
    {
        foreach (RunnerPlayer.PlayerStateAAudioWrapperKVP kvp in kvpList)
        {
            if (kvp.key == keyIndex)
            {
                return kvp.value;
            }
        }
        return null;
    }

    //TODO WE KNOW that we can use interfaces and generics to avoid this duplicated code but we haven't yet :)

    public static bool Contains(this List<RunnerPlayer.HurtObjectAAudioWrapperKVP> kvpList, TerrObject.HAZ_TYPE keyIndex)
    {
        foreach (RunnerPlayer.HurtObjectAAudioWrapperKVP kvp in kvpList)
        {
            if (kvp.key == keyIndex)
            {
                return true;
            }
        }
        return false;
    }

    public static AAudioWrapper GetAAW(this List<RunnerPlayer.HurtObjectAAudioWrapperKVP> kvpList, TerrObject.HAZ_TYPE keyIndex)
    {
        foreach (RunnerPlayer.HurtObjectAAudioWrapperKVP kvp in kvpList)
        {
            if (kvp.key == keyIndex)
            {
                return kvp.value;
            }
        }
        return null;
    }
}

public class RunnerPlayer : RunnerGameObject
{
    const int DODGE_FRAME_DELAY = 1;
    const float SPRINT_TIME = 1.5f;
    const float SPRINT_COOL_DOWN_TIME = 3f;
    const float SPRINT_SPEED_PERCENT_BOOST = 0.3f;
    const float minCollideDist = 1f;

    public const int AMO_SIZE = 4;
    //List<TerrObject> checkClipObjs = new List<TerrObject>();

    //const int JUMP_FRAME_DELAY = 2;

    [SerializeField]
    private BoolPropertySO invincibleSO = null;
    [SerializeField]
    private IntPropertySO livesSO = null;
    [SerializeField]
    private FloatPropertySO livesRegenSO = null;

    public int startingLives;
    private static int lives;
    public static int Lives
    {
        get { return lives; }
        private set
        {
            lives = value;
            ChangedLifeCount(value);
        }
    }

    float lifeRecoverTotalTime = 0f;

    bool canChange = true;
    bool canSprint = true;
    Coroutine sprintCoroutine;
    Coroutine sprintCoolDownCoroutine;
    public bool canChangeState() { return canChange; }

    public bool HasRock { get; set; } = false;
    public static bool HasGun { get; private set; } = false;

    private static int gunBullets = 0;
    public static int GunBullets
    {
        get => gunBullets;

        set
        {
            if (value > 0)
            {
                HasGun = true;
            }
            gunBullets = value;
        }

    }

    Dictionary<string, AnimationClip> animDict = new Dictionary<string, AnimationClip>();

    [Serializable]
    public class PlayerStateAAudioWrapperKVP
    {
        [SerializeField]
        public PLAYER_STATE key = PLAYER_STATE.NONE;
        [SerializeField]
        public AAudioWrapper value;

        public override string ToString()
        {
            return key.ToString();
        }
    }

    //TODO Same here with the duplicated code in these classes
    [Serializable]
    public class HurtObjectAAudioWrapperKVP
    {
        [SerializeField]
        public TerrObject.HAZ_TYPE key = TerrObject.HAZ_TYPE.NONE;
        [SerializeField]
        public AAudioWrapper value;

        public override string ToString()
        {
            return key.ToString();
        }
    }

    [SerializeField]
    private List<PlayerStateAAudioWrapperKVP> playerMovementAudioDict = new List<PlayerStateAAudioWrapperKVP>();

    [SerializeField]
    private List<HurtObjectAAudioWrapperKVP> playerHurtAudioDict = new List<HurtObjectAAudioWrapperKVP>();

    [SerializeField]
    private AAudioWrapper lazerHitSound = null;

    public GameObject LA;
    public GameObject RA;

    protected Animator animLA;
    protected AnimatorOverrideController animLAOC;
    protected string animLAIndex;
    public AnimationClip[] animLAClips;
    Dictionary<string, AnimationClip> animLADict = new Dictionary<string, AnimationClip>();
    void gunFireAnimEnded() => onAnimStateEnd(PLAYER_STATE.FIRE);

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
    public static event System.Action<int> ChangedLifeCount = delegate { };
    //public static event System.Action<TerrObject> removeTerrObj
 
    // Update is called once per frame
    private void Start()
    {

        Lives = startingLives;

        foreach (AnimationClip animClip in animClips)
        {
            //we still want the torso run animation for firing a gun
            if (animClip.name == "RUN")
            {
                animDict.Add("FIRE", animClip);
            }
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
        RunnerGunFire.onGunFireAnimEnd += gunFireAnimEnded;
        RunnerGunFire.onGunFired += generateThrowable;
        RunnerGunFire.onGunThrow += generateThrowable;

        animRAOC = new AnimatorOverrideController(animRA.runtimeAnimatorController);
        animRA.runtimeAnimatorController = animRAOC;
        animRAIndex = animRAOC.animationClips[0].name;

        switchAnimState(PLAYER_STATE.RUN);
    }



    private void OnTriggerEnter(Collider coll)
    {
        if (GameIsOver())
        {
            return;
        }

        TerrObject terrObj = coll.gameObject.GetComponent<TerrObject>();
        RunnerThrowable enemyBullet = coll.gameObject.GetComponent<RunnerThrowable>();

        if (enemyBullet != null
            && enemyBullet.throwType == RunnerThrowable.THROW_TYPE.ENEMY_BULLET
            && currState != PLAYER_STATE.ROLL && !invincibleSO.Value )
        {
            RunnerSounds.Current.PlayAudioWrapper(lazerHitSound, gameObject);
            looseLife(PLAYER_STATE.ROLL);
            //Debug.Log("IM SHOT!!!!");
            return;
        }

        if (terrObj == null) { return;  }

        if (terrObj.objType == TerrObject.OBJ_TYPE.ROCK)
        {
            if (terrObj.actionNeeded == currState)
            {
                GrabbedRock(terrObj);
            }
            return;
        }

        if (terrObj.objType == TerrObject.OBJ_TYPE.TEMP_GUN)
        {
            if (terrObj.actionNeeded == currState)
            {
                GotGun(terrObj);
            }
            return;
        }

        if ( invincibleSO.Value )
        {
            return;
        }

        if (terrObj.objType == TerrObject.OBJ_TYPE.ENEMY)
        {
            if (!terrObj.played)
            {
                return;
            }

            //correct condition for take down
            if (terrObj.actionNeeded == currState && currState == PLAYER_STATE.SPRINT)
            {
                int randomTakedown = Random.Range(0, 3);
                switch (randomTakedown)
                {
                    case 0:
                        defaultInitAction(PLAYER_STATE.TAKEDOWN1);
                        break;
                    case 1:
                        defaultInitAction(PLAYER_STATE.TAKEDOWN2);
                        break;
                    default:
                        defaultInitAction(PLAYER_STATE.TAKEDOWN3);
                        break;
                }
                //only get the gun if its an alien shooter (which always has an
                //alien shooter script attached to it)
                if (terrObj.GetComponent<AlienShooter>() != null)
                {
                    GotGun();
                }
                
                return;
            }
        }

        if (terrObj.objType == TerrObject.OBJ_TYPE.STATIC || terrObj.actionNeeded == currState)
        {
            return;
        }

        looseLife(terrObj);
        //Debug.Log("object: " + terrObj.name);
        //Debug.Log("IM HIT!!!!");

    }

    void GrabbedRock(TerrObject rockObj)
    {
        if (HasRock) { return; }

        //Debug.Log("GOT ROCK!");

        HasRock = true;

        //make rock "disappear"
        rockObj.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
    }

    void GotGun(TerrObject tempGunObj = null)
    {
        //if (GunBullets > 0) { return; }

        //Debug.Log("GOT GUN!");

        GunBullets = AMO_SIZE;

        if (tempGunObj == null) { return; }

        tempGunObj.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
    }

    private void looseLife(TerrObject hazObject)
    {
        if (playerHurtAudioDict.Contains(hazObject.hazType))
        {
            RunnerSounds.Current.PlayAudioWrapper(playerHurtAudioDict.GetAAW(hazObject.hazType), gameObject);
        }
        looseLife(hazObject.actionNeeded);
    }
    private void looseLife(PLAYER_STATE stateThatWasNeeded)
    {
        if (currState == PLAYER_STATE.DEATH1
            || currState == PLAYER_STATE.HURT_F
            || currState == PLAYER_STATE.HURT_L
            || currState == PLAYER_STATE.HURT_T)
        {
            return;
        }

        livesSO.ModifyValue( -1 );

        Lives--;
        lifeRecoverTotalTime = 0f;

        PLAYER_STATE state = PLAYER_STATE.NONE;

        switch (stateThatWasNeeded)
        {
            case PLAYER_STATE.NONE:
            case PLAYER_STATE.SPRINT:
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
        if (Lives >= startingLives) { return; }

        if (GameIsOver()) { return; }

        lifeRecoverTotalTime += Time.deltaTime;

        if (lifeRecoverTotalTime >= livesRegenSO.Value )
        {
            livesSO.ModifyValue( 1 );
            Lives++;
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
        changeTime = GameIsOver() && treadmillOn && currState != PLAYER_STATE.DEATH1? changeTime * 4 : changeTime;
        changeTreamillSpeed(treadmillOn, changeTime, speedMultiplyer);
    }

    bool GameIsOver()
    {
        return Lives < 0;
    }

    void playAnimAudioWrapper(AAudioWrapper aw)
    {
        RunnerSounds.Current.PlayAudioWrapper(aw, gameObject);
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

        onAnimStateEnd(animState);
    }

    //So that we can tell it fire ended without an anim clip
    void onAnimStateEnd(PLAYER_STATE animState)
    {
        if (GameIsOver() && animState != PLAYER_STATE.DEATH1)
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
                onAnimEndSwitch();
                break;


            case PLAYER_STATE.LAND_G:
                canChange = true;
                switchAnimState(PLAYER_STATE.RUN);
                break;


            case PLAYER_STATE.DODGE_R:
            case PLAYER_STATE.ROLL:
                switchAnimState(PLAYER_STATE.RUN, 7);
                onAnimEndSwitch();
                break;

            case PLAYER_STATE.FIRE:
                switchAnimState(PLAYER_STATE.RUN, getCurrFrame(animDict["RUN"], anim));
                onAnimEndSwitch();
                break;

            default:
                switchAnimState(PLAYER_STATE.RUN);
                onAnimEndSwitch();
                break;
        }


        onAnimationEnded(animState);
    }

    private void onAnimEndSwitch()
    {
        canChange = true;
        //in case we switched from a sprinting anim
        if (!canSprint && sprintCoolDownCoroutine == null)
        {
            sprintCoolDownCoroutine = StartCoroutine(sprintCoolDown());
        }
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
        if (!HasRock) { return; }
        defaultInitAction(PLAYER_STATE.THROW_R);
        HasRock = false;
    }

    public void fireGun()
    {
        if (!HasGun)
        {
            return;
        }

        if (GunBullets == 0)
        {
            HasGun = false;
            defaultInitAction(PLAYER_STATE.THROW_G);
            return;
        }

        GunBullets--;

        defaultInitAction(PLAYER_STATE.FIRE);
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
        PlayAAudioWrapper(state);
        StartCoroutine(switchAnim(state, startFrame));
    }

    private void PlayAAudioWrapper(PLAYER_STATE state)
    {
        if (playerMovementAudioDict.Contains(state))
        {
            RunnerSounds.Current.PlayAudioWrapper(playerMovementAudioDict.GetAAW(state), this.gameObject);
        }
    }

    IEnumerator switchAnim(PLAYER_STATE state, int startFrame = 0)
    {
        onAnimationStarted(state);


        string stateString = state.ToString();


        int currState = anim.GetCurrentAnimatorStateInfo(0).fullPathHash;

        //only exception to using startFrame with firing gun
        int torsoStartFrame = state == PLAYER_STATE.FIRE? getCurrFrame(animDict["RUN"], anim) : startFrame;
        float startNormTime = getNormTimeFromFrame(animDict[stateString], anim, torsoStartFrame);

        
        

        string LAstring = "LA_" + stateString;
        LAstring = HasGun && state != PLAYER_STATE.THROW_G && state != PLAYER_STATE.FIRE? LAstring + "_GUN" : LAstring;
        int currStateLA = animLA.GetCurrentAnimatorStateInfo(0).fullPathHash;
        float startNormTimeLA = getNormTimeFromFrame(animLADict[LAstring], animLA, startFrame);
        

        string RAstring = "RA_" + stateString;
        //TODO: get firing anim while holding rock
        RAstring = HasRock && state != PLAYER_STATE.THROW_R && state != PLAYER_STATE.FIRE? RAstring + "_ROCK" : RAstring;
        
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
        throwablesDict[throwType].Instantiate(transform.position);
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
