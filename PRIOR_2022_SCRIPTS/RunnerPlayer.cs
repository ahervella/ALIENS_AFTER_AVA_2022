using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public static class PSAAW_ExtensionMethods
{

    public static bool Contains(this List<RunnerPlayer.PlayerStateAAudioWrapperKVP> kvpList, RunnerGameObject.PLAYER_STATE keyIndex)
    {
        foreach (RunnerPlayer.PlayerStateAAudioWrapperKVP kvp in kvpList)
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

    private float lifeRecoverTotalTime = 0f;

    private bool canChange = true;
    private bool canSprint = true;
    private Coroutine sprintCoroutine;
    private Coroutine sprintCoolDownCoroutine;
    public bool CanChangeState() { return canChange; }

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
    private void GunFireAnimEnded() => OnAnimStateEnd(PLAYER_STATE.FIRE);

    protected Animator animRA;
    protected AnimatorOverrideController animRAOC;
    protected string animRAIndex;
    public AnimationClip[] animRAClips;
    Dictionary<string, AnimationClip> animRADict = new Dictionary<string, AnimationClip>();


    public RunnerThrowable[] throwables;
    Dictionary<RunnerThrowable.THROW_TYPE, RunnerThrowable> throwablesDict = new Dictionary<RunnerThrowable.THROW_TYPE, RunnerThrowable>();

    PLAYER_STATE currState = PLAYER_STATE.RUN;


    public static event System.Action<PLAYER_STATE> OnAnimationStarted = delegate { };
    public static event System.Action<PLAYER_STATE> OnAnimationEnded = delegate { };
    public static event System.Action<bool, float, float> ChangeTreamillSpeed = delegate { };
	
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
        RunnerGunFire.OnGunFireAnimEnd += GunFireAnimEnded;
        RunnerGunFire.OnGunFired += GenerateThrowable;
        RunnerGunFire.OnGunThrow += GenerateThrowable;

        animRAOC = new AnimatorOverrideController(animRA.runtimeAnimatorController);
        animRA.runtimeAnimatorController = animRAOC;
        animRAIndex = animRAOC.animationClips[0].name;

        SwitchAnimState(PLAYER_STATE.RUN);
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
            LooseLife(PLAYER_STATE.ROLL);
            //Debug.Log("IM SHOT!!!!");
            return;
        }

        if (terrObj == null) { return; }

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
                        DefaultInitAction(PLAYER_STATE.TAKEDOWN1);
                        break;
                    case 1:
                        DefaultInitAction(PLAYER_STATE.TAKEDOWN2);
                        break;
                    default:
                        DefaultInitAction(PLAYER_STATE.TAKEDOWN3);
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

        LooseLife(terrObj);
        //Debug.Log("object: " + terrObj.name);
        //Debug.Log("IM HIT!!!!");

    }

    private void GrabbedRock(TerrObject rockObj)
    {
        if (HasRock) { return; }

        //Debug.Log("GOT ROCK!");

        HasRock = true;

        //make rock "disappear"
        rockObj.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
    }

    private void GotGun(TerrObject tempGunObj = null)
    {
        //if (GunBullets > 0) { return; }

        //Debug.Log("GOT GUN!");

        GunBullets = AMO_SIZE;

        if (tempGunObj == null) { return; }

        tempGunObj.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
    }

    private void LooseLife(TerrObject hazObject)
    {
        if (playerHurtAudioDict.Contains(hazObject.hazType))
        {
            RunnerSounds.Current.PlayAudioWrapper(playerHurtAudioDict.GetAAW(hazObject.hazType), gameObject);
        }
        LooseLife(hazObject.actionNeeded);
    }
    private void LooseLife(PLAYER_STATE stateThatWasNeeded)
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

        DefaultInitAction(state);

    }


    private void Update()
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


    /// <summary>
    /// Used by animation events
    /// </summary>
    /// <param name="aw"></param>
    private void treadmillOff(int changeTimeInFrames) { TreadmillSpeedChange(false, changeTimeInFrames); }

    /// <summary>
    /// Used by animation events
    /// </summary>
    /// <param name="aw"></param>
    private void treadmillOn(int changeTimeInFrames) { TreadmillSpeedChange(true, changeTimeInFrames); }

    private void TreadmillSpeedChange(bool treadmillOn, int changeTimeInFrames, float speedMultiplyer = 1f)//, int changeTimeInFrames)
    {
        float changeTime = GetTimeFromFrames(animDict[currState.ToString()], changeTimeInFrames);
        //for smoother animation treadmill speed change
        changeTime = GameIsOver() && treadmillOn && currState != PLAYER_STATE.DEATH1 ? changeTime * 4 : changeTime;
        ChangeTreamillSpeed(treadmillOn, changeTime, speedMultiplyer);
    }

    private bool GameIsOver()
    {
        return Lives < 0;
    }

    /// <summary>
    /// Used by animation events
    /// </summary>
    /// <param name="aw"></param>
    private void playAnimAudioWrapper(AAudioWrapper aw)
    {
        RunnerSounds.Current.PlayAudioWrapper(aw, gameObject);
    }

    private void onAnimEnd(AnimationClip animClip)
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

        OnAnimStateEnd(animState);
    }

    //So that we can tell it fire ended without an anim clip
    private void OnAnimStateEnd(PLAYER_STATE animState)
    {
        if (GameIsOver() && animState != PLAYER_STATE.DEATH1)
        {
            InitDeath();
            OnAnimationEnded(animState);
            return;
        }

        switch (animState)
        {
            case PLAYER_STATE.DEATH1:
                return;

            case PLAYER_STATE.JUMP:
                SwitchAnimState(PLAYER_STATE.LAND_G);
                //controls feel better this way
                OnAnimEndSwitch();
                break;


            case PLAYER_STATE.LAND_G:
                canChange = true;
                SwitchAnimState(PLAYER_STATE.RUN);
                break;


            case PLAYER_STATE.DODGE_R:
            case PLAYER_STATE.ROLL:
                SwitchAnimState(PLAYER_STATE.RUN, 7);
                OnAnimEndSwitch();
                break;

            case PLAYER_STATE.FIRE:
                SwitchAnimState(PLAYER_STATE.RUN, GetCurrFrame(animDict["RUN"], anim));
                OnAnimEndSwitch();
                break;

            default:
                SwitchAnimState(PLAYER_STATE.RUN);
                OnAnimEndSwitch();
                break;
        }


        OnAnimationEnded(animState);
    }

    private void OnAnimEndSwitch()
    {
        canChange = true;
        //in case we switched from a sprinting anim
        if (!canSprint && sprintCoolDownCoroutine == null)
        {
            sprintCoolDownCoroutine = StartCoroutine(SprintCoolDown());
        }
    }


    public void InitDeath()
    {
        SwitchAnimState(PLAYER_STATE.DEATH1);
    }


    public float Dodge(bool dodgeRight)
    {
        float delayFromDodge = 0;

        if (dodgeRight)
        {
            DefaultInitAction(PLAYER_STATE.DODGE_R);
            delayFromDodge = GetNormTimeFromFrame(animDict[PLAYER_STATE.DODGE_R.ToString()], anim, DODGE_FRAME_DELAY);
        }
        else
        {
            DefaultInitAction(PLAYER_STATE.DODGE_L);
            delayFromDodge = GetNormTimeFromFrame(animDict[PLAYER_STATE.DODGE_L.ToString()], anim, DODGE_FRAME_DELAY);
        }

        return delayFromDodge;

    }

    public void Jump()
    {
        DefaultInitAction(PLAYER_STATE.JUMP);

    }

    public void Roll()
    {
        DefaultInitAction(PLAYER_STATE.ROLL);
    }



    public void Sprint()
    {
        if (!canSprint || !canChange) { return; }
        SwitchAnimState(PLAYER_STATE.SPRINT);
        TreadmillSpeedChange(true, 6, 1f + SPRINT_SPEED_PERCENT_BOOST);
        sprintCoroutine = StartCoroutine(SprintInitTimer());
    }

    private IEnumerator SprintInitTimer()
    {
        canSprint = false;
        yield return new WaitForSecondsRealtime(SPRINT_TIME);
        SwitchAnimState(PLAYER_STATE.RUN);
        sprintCoroutine = null;
        sprintCoolDownCoroutine = StartCoroutine(SprintCoolDown());
    }

    private IEnumerator SprintCoolDown()
    {
        OnAnimationEnded(PLAYER_STATE.SPRINT);
        TreadmillSpeedChange(true, 6);
        yield return new WaitForSecondsRealtime(SPRINT_COOL_DOWN_TIME);
        canSprint = true;
        sprintCoolDownCoroutine = null;

    }



    public void ThrowRock()
    {
        if (!HasRock) { return; }
        DefaultInitAction(PLAYER_STATE.THROW_R);
        HasRock = false;
    }

    public void FireGun()
    {
        if (!HasGun)
        {
            return;
        }

        if (GunBullets == 0)
        {
            HasGun = false;
            DefaultInitAction(PLAYER_STATE.THROW_G);
            return;
        }

        GunBullets--;

        DefaultInitAction(PLAYER_STATE.FIRE);
    }


    private void DefaultInitAction(PLAYER_STATE state)
    {
        SwitchAnimState(state);
        canChange = false;
        if (sprintCoroutine != null) { StopCoroutine(sprintCoroutine); sprintCoroutine = null; }
    }

    private void SwitchAnimState(PLAYER_STATE state, int startFrame = 0)
    {
        currState = state;
        PlayAAudioWrapper(state);
        StartCoroutine(SwitchAnim(state, startFrame));
    }

    private void PlayAAudioWrapper(PLAYER_STATE state)
    {
        if (playerMovementAudioDict.Contains(state))
        {
            RunnerSounds.Current.PlayAudioWrapper(playerMovementAudioDict.GetAAW(state), this.gameObject);
        }
    }

    private IEnumerator SwitchAnim(PLAYER_STATE state, int startFrame = 0)
    {
        OnAnimationStarted(state);


        string stateString = state.ToString();


        int currState = anim.GetCurrentAnimatorStateInfo(0).fullPathHash;

        //only exception to using startFrame with firing gun
        int torsoStartFrame = state == PLAYER_STATE.FIRE ? GetCurrFrame(animDict["RUN"], anim) : startFrame;
        float startNormTime = GetNormTimeFromFrame(animDict[stateString], anim, torsoStartFrame);




        string LAstring = "LA_" + stateString;
        LAstring = HasGun && state != PLAYER_STATE.THROW_G && state != PLAYER_STATE.FIRE ? LAstring + "_GUN" : LAstring;
        int currStateLA = animLA.GetCurrentAnimatorStateInfo(0).fullPathHash;
        float startNormTimeLA = GetNormTimeFromFrame(animLADict[LAstring], animLA, startFrame);


        string RAstring = "RA_" + stateString;
        //TODO: get firing anim while holding rock
        RAstring = HasRock && state != PLAYER_STATE.THROW_R && state != PLAYER_STATE.FIRE ? RAstring + "_ROCK" : RAstring;

        int currStateRA = animRA.GetCurrentAnimatorStateInfo(0).fullPathHash;
        float startNormTimeRA = GetNormTimeFromFrame(animRADict[RAstring], animRA, startFrame);

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



    public void GenerateThrowable(RunnerThrowable.THROW_TYPE throwType)
    {
        throwablesDict[throwType].Instantiate(transform.position);
    }

    private int GetCurrFrame(AnimationClip animClip, Animator animator)
    {
        float normTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        float animLength = animClip.length;
        float animFrameRate = animClip.frameRate;
        return (int)(normTime * animLength * animFrameRate);
    }

    private float GetTimeFromFrames(AnimationClip animClip, int frameCount)
    {
        return animClip.frameRate / RunnerGameObject.GetGameFPS() * frameCount;
    }

    private float GetNormTimeFromFrame(AnimationClip animClip, Animator animator, int frame)
    {
        float normTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        float animLength = animClip.length;
        float animFrameRate = animClip.frameRate;
        return (float)frame / (animLength * animFrameRate);
    }
}
